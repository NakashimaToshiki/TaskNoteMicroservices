using AutoMapper;
using Job.Entity.FrameworkCore;
using Job.Entity.FrameworkCore.Mapper;
using Job.Entity.FrameworkCore.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

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

// 下記の接続文字はAuzre上のデータベースをVisualStudioのNugetでマイグレーションしたい場合にコメントインします。
// 正しい方法はCI/CDでデプロイ時にデータベースのマイグレーションが発生すべきなので一時的な利用方法として認識して下さい。
//optionsBuilder.UseSqlServer("@Data Source=tasknote.database.windows.net;Initial Catalog=TaskNote.Server_db;User ID=admin;Password=abcdefg");

// あとで設定ファイルから読み込めるようにする
//optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Database"));
//builder.Services.AddDbContextFactory<JobDbContext>(options => options.UseSqlServer(configuration))

builder.Services.AddDbContextFactory<JobDbContext>(options =>
{
    options.UseInMemoryDatabase("dammy");
});


// 今のところどのWebサイトからでもWebApiにアクセスできるように設定している
// リリースする際は必ず制限する
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

// 出力されるHTMLの日本語文字が実態参照(数値文字参照)で出力されないようにする
builder.Services.Configure<WebEncoderOptions> (options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});

var app = builder.Build();

// データベースの初期値生成のために必要
var dbContextFactory = app.Services.GetService<IDbContextFactory<JobDbContext>>();
var db = dbContextFactory?.CreateDbContext();
db?.Database.EnsureCreated();

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
