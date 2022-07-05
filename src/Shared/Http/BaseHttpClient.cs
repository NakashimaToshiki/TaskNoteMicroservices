using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using System.Net.Http.Json;

namespace Shared.Http;

public abstract class BaseApiClient<Model> where Model : class
{
    private readonly HttpClient _client;

    public BaseApiClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }


    protected async Task<Model?> Get(string requestUri)
    {
        var response = await _client.GetAsync(requestUri);

        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadFromJsonAsync<Model>();
                    case HttpStatusCode.NoContent: // 主に検索結果で何もデータが無かった場合
                        return null;
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4: // クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        if (mediaType == MideiaTypeNamesExtentions.Application.ProblemJson)
                        {
                            return null; // Urlは存在するがリソースは存在しない
                        }
                        else
                        {
                            throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                        }
                    case HttpStatusCode.BadRequest:
                    default:
                        throw new BadRequestException(
                            mediaType == MediaTypeNames.Text.Plain ? await response.Content.ReadAsStringAsync() : string.Empty,
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                }
            case 5: // サーバーエラーレスポンス
                throw new HttpRequestException($"サーバーエラー\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
            default:
                throw new HttpRequestException($"予期しないステータスコード\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }

    protected async Task<IEnumerable<Model>> Search<JobSearchModel>(string requestUri, JobSearchModel searchModel)
    {
        var response = await _client.PostAsJsonAsync(requestUri, searchModel);

        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadFromJsonAsync<IEnumerable<Model>>() ?? new List<Model>();
                    case HttpStatusCode.NoContent:
                        return new List<Model>();
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4: // クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                    case HttpStatusCode.BadRequest:
                    default:
                        throw new BadRequestException(
                            mediaType == MediaTypeNames.Text.Plain ? await response.Content.ReadAsStringAsync() : string.Empty,
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                }
            case 5: // サーバーエラーレスポンス
                throw new HttpRequestException($"サーバーエラー\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
            default:
                throw new HttpRequestException($"予期しないステータスコード\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }

    protected async Task<Model?> Create(string requestUri, Model input)
    {
        var response = await _client.PostAsJsonAsync(requestUri, input);
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Created:
                        return await response.Content.ReadFromJsonAsync<Model>();
                    case HttpStatusCode.NoContent:
                        return null;
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4:// クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                    case HttpStatusCode.Conflict:
                        throw new BadRequestException(
                            "既に存在するデータです。",
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                    case HttpStatusCode.BadRequest:
                    default:
                        throw new BadRequestException(
                            mediaType == MediaTypeNames.Text.Plain ? await response.Content.ReadAsStringAsync() : string.Empty,
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                }
            case 5: // サーバーエラーレスポンス
                throw new HttpRequestException($"サーバーエラー\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
            default:
                throw new HttpRequestException($"予期しないステータスコード\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }

    /// <summary>
    /// リソースを追加する。既に存在する場合は更新する。
    /// </summary>
    /// <param name="input"></param>
    /// <returns>更新した場合はfalse 追加した場合はtrue</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="BadRequestException"></exception>
    protected async Task<(Model?, bool)> CreateOrUpdate(string requestUri, Model input)
    {
        var response = await _client.PutAsJsonAsync(requestUri, input);
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return (await response.Content.ReadFromJsonAsync<Model>(), false);
                    case HttpStatusCode.Created:
                        return (await response.Content.ReadFromJsonAsync<Model>(), true);
                    case HttpStatusCode.NoContent:
                        return (null, false);
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4:// クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                    case HttpStatusCode.Conflict:
                        throw new BadRequestException(
                            "既存のリソースを更新できませんでした。",
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                    case HttpStatusCode.BadRequest:
                    default:
                        throw new BadRequestException(
                            mediaType == MediaTypeNames.Text.Plain ? await response.Content.ReadAsStringAsync() : string.Empty,
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                }
            case 5: // サーバーエラーレスポンス
                throw new HttpRequestException($"サーバーエラー\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
            default:
                throw new HttpRequestException($"予期しないステータスコード\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }

    protected async Task<Model?> Update(string requestUri, Model input)
    {
        var response = await _client.PatchAsync(requestUri, JsonContent.Create(input));
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Created:
                        return await response.Content.ReadFromJsonAsync<Model>();
                    case HttpStatusCode.NoContent:
                        return null;
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4:// クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                    case HttpStatusCode.Conflict:
                        throw new BadRequestException(
                            "既に存在するデータです。",
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                    case HttpStatusCode.BadRequest:
                    default:
                        throw new BadRequestException(
                            mediaType == MediaTypeNames.Text.Plain ? await response.Content.ReadAsStringAsync() : string.Empty,
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                }
            case 5: // サーバーエラーレスポンス
                throw new HttpRequestException($"サーバーエラー\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
            default:
                throw new HttpRequestException($"予期しないステータスコード\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }

    protected async Task<bool> Delete(string requestUri)
    {
        var response = await _client.DeleteAsync(requestUri);

        var mediaType = response.Content.Headers?.ContentType?.MediaType;

        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        return true;
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4: // クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        if (mediaType == MideiaTypeNamesExtentions.Application.ProblemJson)
                        {
                            return false; // Urlは存在するがリソースは存在しない
                        }
                        else
                        {
                            throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                        }
                    case HttpStatusCode.BadRequest:
                    default:
                        throw new BadRequestException(
                            mediaType == MediaTypeNames.Text.Plain ? await response.Content.ReadAsStringAsync() : string.Empty,
                            mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                            );
                }
            case 5: // サーバーエラーレスポンス
                throw new HttpRequestException($"サーバーエラー\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
            default:
                throw new HttpRequestException($"予期しないレスポンス\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }
}
