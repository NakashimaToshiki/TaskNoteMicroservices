using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Http;

public record BadResponse
{
    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int Status { get; set; }

    public string TraceId { get; set; } = string.Empty;

    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
}

public record NullBadReponse : BadResponse
{
    private readonly static string message = "サーバーから詳細なエラーを取得できませんでした";

    public static BadResponse Instance { get; } = new BadResponse()
    {
        Type = message,
        Title = message,
        Status = 0,
        TraceId = message,
    };

    protected NullBadReponse()
    {
    }
}

/*
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-8b36b5e0481e7ea6bd380bff90563f3b-308a47108922734d-00",
  "errors": {
    "UserId": [
      "The UserId field is required."
    ]
  }
}
    */

/*
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-1aa391d3ac3737a9f966d0bdef91fec6-0a79e5cc5c1fafd8-00",
  "errors": {
    "$": [
      "The JSON object contains a trailing comma at the end which is not supported in this mode. Change the reader options. Path: $ | LineNumber: 8 | BytePositionInLine: 0."
    ],
    "input": [
      "The input field is required."
    ]
  }
}
 * */