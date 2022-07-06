using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using System.Net.Http.Json;

namespace Shared.Http;

public abstract class BaseApiClient
{
    private readonly HttpClient _client;

    public BaseApiClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }


    protected async Task<TOutput?> Get<TOutput>(string requestUri) where TOutput : class
    {
        var response = await _client.GetAsync(requestUri);

        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadFromJsonAsync<TOutput>();
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

    protected async Task<IEnumerable<TOutput>> Search<TOutput, JobSearchModel>(string requestUri, JobSearchModel searchModel) where TOutput : class
    {
        var response = await _client.PostAsJsonAsync(requestUri, searchModel);

        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadFromJsonAsync<IEnumerable<TOutput>>() ?? new List<TOutput>();
                    case HttpStatusCode.NoContent:
                        return new List<TOutput>();
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

    protected async Task<TOutput?> CreateEcho<TOutput, TInput>(string requestUri, TInput input) where TOutput : class
    {
        var response = await _client.PostAsJsonAsync(requestUri, input);
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (mediaType == MediaTypeNames.Application.Json)
            {
                return await response.Content.ReadFromJsonAsync<TOutput>();
            }
            else
            {
                return null;
            }
        }
        if (!await SubCreate(response))
        {
            throw new BadRequestException(
                "既に存在するデータです。",
                mediaType == MideiaTypeNamesExtentions.Application.ProblemJson ? await response.Content.ReadFromJsonAsync<BadResponse>() ?? NullBadReponse.Instance : NullBadReponse.Instance
                );
        }
        return null;
    }


    protected async Task<bool> Create<TInput>(string requestUri, TInput input)
    {
        var response = await _client.PostAsJsonAsync(requestUri, input);
        return await SubCreate(response);
    }

    protected async Task<bool> SubCreate(HttpResponseMessage response)
    {
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:
                        return true;
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4:// クライアントエラーレスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException($"urlが間違っている Uri = {response.RequestMessage?.RequestUri}");
                    case HttpStatusCode.Conflict:
                        return false;
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
    protected async Task<(TOutput?, bool)> CreateOrUpdateEcho<TOutput, TInput>(string requestUri, TInput input) where TOutput : class
    {
        var response = await _client.PutAsJsonAsync(requestUri, input);
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return (await response.Content.ReadFromJsonAsync<TOutput>(), false);
            case HttpStatusCode.Created:
                if (mediaType == MediaTypeNames.Application.Json)
                {
                    return (await response.Content.ReadFromJsonAsync<TOutput>(), true);
                }
                else
                {
                    return (null, true);
                }
            default:
                var ret = await SubCreateOrUpdate(response);
                return (null, ret);
        }
    }

    /// <summary>
    /// リソースを追加する。既に存在する場合は更新する。
    /// </summary>
    /// <param name="input"></param>
    /// <returns>更新した場合はfalse 追加した場合はtrue</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="BadRequestException"></exception>
    protected async Task<bool> CreateOrUpdate<TInput>(string requestUri, TInput input)
    {
        var response = await _client.PutAsJsonAsync(requestUri, input);
        return await SubCreateOrUpdate(response);
    }

    protected async Task<bool> SubCreateOrUpdate(HttpResponseMessage response)
    {
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK: // 更新した場合
                        return false;
                    case HttpStatusCode.Created:
                        return true;
                    case HttpStatusCode.NoContent: // 更新した場合
                        return false;
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


    /// <summary>
    /// リソースを更新する
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="requestUri"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    protected async Task<TOutput?> UpdateEcho<TOutput, TInput>(string requestUri, TInput input) where TOutput : class
    {
        var response = await _client.PatchAsync(requestUri, JsonContent.Create(input));

        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return await response.Content.ReadFromJsonAsync<TOutput>();
        }
        else
        {
            var ret = await SubUpdate(response);
            return null;
        }
    }

    /// <summary>
    /// リソースを更新する
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="requestUri"></param>
    /// <param name="input">更新データ</param>
    /// <returns>対象のリソースが存在しない場合はfalse</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="BadRequestException"></exception>
    protected async Task<bool> Update<TInput>(string requestUri, TInput input)
    {
        var response = await _client.PatchAsync(requestUri, JsonContent.Create(input));
        return await SubUpdate(response);
    }

    private async Task<bool> SubUpdate(HttpResponseMessage response)
    {
        var mediaType = response.Content.Headers?.ContentType?.MediaType;
        switch ((int)response.StatusCode / 100)
        {
            case 2: // 成功レスポンス
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:
                        return true;
                    default:
                        throw new HttpRequestException("予期しないステータスコード", null, response.StatusCode);
                }
            case 4:// クライアントエラーレスポンス
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
                throw new HttpRequestException($"予期しないステータスコード\r\n{await response.Content.ReadAsStringAsync()}", null, response.StatusCode);
        }
    }

    /// <summary>
    /// リソースを削除する
    /// </summary>
    /// <param name="requestUri">リソース</param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="BadRequestException"></exception>
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
