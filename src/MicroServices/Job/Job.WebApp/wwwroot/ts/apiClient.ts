class JobClient {

    _scheme: string = 'api/job';

    _contentType: string = 'Content-type';

    _problemJson: string = "application/problem+json";

    _plain: string = "text/plain";

    constructor() {
    }

    async getById(path: string, params: string) {
        var uri = this.getHref(path, params);
        let response = await fetch(uri, {
            method: 'GET',
        });
        var mediaType = response.headers.get(this._contentType);
        switch (response.status / 100) {
            case 2:
                switch (response.status) {
                    case 200:
                        let data = await response.json();
                        return data;
                    case 204:
                        return null;
                    default:
                        throw new HttpRequestError(response.status, "予期しないステータスコード");
                }
            case 4:
                switch (response.status)
                {
                    case 404:
                        if (mediaType == this._problemJson) {
                            return null;
                        } else {
                            throw new HttpRequestError(response.status, "urlが間違っている Uri = " + uri);
                        }
                    case 400:
                    default:
                        throw new BadRequestError(
                            mediaType == this._problemJson ? await response.json() : null,
                            response.status,
                            mediaType == this._plain ? await response.text() : ""
                        );
                }
            case 5:
                throw new HttpRequestError(response.status, "サーバーエラー" + await response.text());
            default:
                throw new HttpRequestError(response.status, "予期しないステータスコード\r\n{" + await response.text());

        }
    }

    /**
     * 資源の所在を生成
     * @param {string} path urlパス
     * @param {any} params リクエストパラメータ
     * @returns {string} url
     */
    getHref(path: string, params : any) {
        var href = this._scheme;
        if (path != undefined && path != '') {
            href += '/' + path;
        }
        if (params != undefined) {
            var search = new URLSearchParams(params);
            href += '?' + search.toString();
        }
        return href;
    }
}

/** 共通のエラークラス */
class BaseError extends Error {
    constructor(e?: string) {
        super(e);
        this.name = new.target.name;
    }
}

/** Http関連のエラー */
class HttpRequestError extends BaseError {
    constructor(public statusCode: number, e?: string) {
        super(e);
    }
}

/** Http関連のエラー */
class BadRequestError extends HttpRequestError {
    constructor(public badResponse : BadResponse, public statusCode: number, e?: string) {
        super(statusCode, e);
    }
}

class BadResponse
{
    public type: string;

    public title : string ;

    public status: number;

    public traceId : string;

    public errors: any;
}

class JobModel {
    id: number;
    title: string;
    description: string;
    isCompleted: string;
}