using AutoMapper;
using Job.Entity.FrameworkCore;
using Job.Entity.FrameworkCore.Mapper;
using Job.Entity.FrameworkCore.Sessions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbSessionServices();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<AutoMapperProfileConfiguration>();
}).AddSingleton<IMapper, Mapper>();

// ���L�̐ڑ�������Auzre��̃f�[�^�x�[�X��VisualStudio��Nuget�Ń}�C�O���[�V�����������ꍇ�ɃR�����g�C�����܂��B
// ���������@��CI/CD�Ńf�v���C���Ƀf�[�^�x�[�X�̃}�C�O���[�V�������������ׂ��Ȃ̂ňꎞ�I�ȗ��p���@�Ƃ��ĔF�����ĉ������B
//optionsBuilder.UseSqlServer("@Data Source=tasknote.database.windows.net;Initial Catalog=TaskNote.Server_db;User ID=admin;Password=abcdefg");

// ���ƂŐݒ�t�@�C������ǂݍ��߂�悤�ɂ���
//optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Database"));
//builder.Services.AddDbContextFactory<JobDbContext>(options => options.UseSqlServer(configuration))

builder.Services.AddDbContextFactory<JobDbContext>(options => options.UseInMemoryDatabase("dammy"));

string _allowSpecificOrigins = "allowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _allowSpecificOrigins,
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();