var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class JobClient {
    constructor() {
        this._scheme = 'api/job';
        this._contentType = 'Content-type';
        this._problemJson = "application/problem+json";
        this._plain = "text/plain";
    }
    getById(path, params) {
        return __awaiter(this, void 0, void 0, function* () {
            var uri = this.getHref(path, params);
            let response = yield fetch(uri, {
                method: 'GET',
            });
            var mediaType = response.headers.get(this._contentType);
            switch (response.status / 100) {
                case 2:
                    switch (response.status) {
                        case 200:
                            let data = yield response.json();
                            return data;
                        case 204:
                            return null;
                        default:
                            throw new HttpRequestError(response.status, "予期しないステータスコード");
                    }
                case 4:
                    switch (response.status) {
                        case 404:
                            if (mediaType == this._problemJson) {
                                return null;
                            }
                            else {
                                throw new HttpRequestError(response.status, "urlが間違っている Uri = " + uri);
                            }
                        case 400:
                        default:
                            throw new BadRequestError(mediaType == this._problemJson ? yield response.json() : null, response.status, mediaType == this._plain ? yield response.text() : "");
                    }
                case 5:
                    throw new HttpRequestError(response.status, "サーバーエラー" + (yield response.text()));
                default:
                    throw new HttpRequestError(response.status, "予期しないステータスコード\r\n{" + (yield response.text()));
            }
        });
    }
    /**
     * 資源の所在を生成
     * @param {string} path urlパス
     * @param {any} params リクエストパラメータ
     * @returns {string} url
     */
    getHref(path, params) {
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
    constructor(e) {
        super(e);
        this.name = new.target.name;
    }
}
/** Http関連のエラー */
class HttpRequestError extends BaseError {
    constructor(statusCode, e) {
        super(e);
        this.statusCode = statusCode;
    }
}
/** Http関連のエラー */
class BadRequestError extends HttpRequestError {
    constructor(badResponse, statusCode, e) {
        super(statusCode, e);
        this.badResponse = badResponse;
        this.statusCode = statusCode;
    }
}
class BadResponse {
}
class JobModel {
}
//# sourceMappingURL=apiClient.js.map