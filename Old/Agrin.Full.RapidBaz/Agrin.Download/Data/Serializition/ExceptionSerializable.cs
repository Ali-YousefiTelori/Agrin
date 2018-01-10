using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class ExceptionSerializable
    {
        public static ExceptionSerializable ExceptionToSerializable(Exception e)
        {
            if (e is WebException)
            {
                var ex = e as WebException;
                List<object> data = new List<object>();
                data.Add(ex.Status);
                if (ex.Response != null && ex.Response is HttpWebResponse)
                {
                    data.Add(((HttpWebResponse)ex.Response).StatusCode);
                    if (!string.IsNullOrEmpty(((HttpWebResponse)ex.Response).StatusDescription))
                        data.Add(((HttpWebResponse)ex.Response).StatusDescription);
                }

                return new ExceptionSerializable() { ExceptionData = e, OtherData = data };
            }
            return new ExceptionSerializable() { ExceptionData = e };
        }
        public Exception ExceptionData { get; set; }
        public List<object> OtherData { get; set; }
        public int Count { get; set; }
    }

    public static class ErrorDescription
    {
        public static string GetErrorMessageByWebExceptionStatus(WebExceptionStatus state)
        {
            if (state == WebExceptionStatus.Success)
                return @"Summary:
             No error was encountered.";
            if (state == WebExceptionStatus.NameResolutionFailure)

                return @"Summary:
             The name resolver service could not resolve the host name.";
            if (state == WebExceptionStatus.ConnectFailure)

                return @"Summary:
             The remote service point could not be contacted at the transport level.";
            if (state == WebExceptionStatus.ReceiveFailure)

                return @"Summary:
             A complete response was not received from the remote server.";
            if (state == WebExceptionStatus.SendFailure)

                return @"Summary:
             A complete request could not be sent to the remote server.";
            if (state == WebExceptionStatus.PipelineFailure)

                return @"Summary:
             The request was a piplined request and the connection was closed before the
             response was received.";
            if (state == WebExceptionStatus.RequestCanceled)

                return @"Summary:
             The request was canceled, the System.Net.WebRequest.Abort() method was called,
             or an unclassifiable error occurred. This is the default value for System.Net.WebException.Status.";
            if (state == WebExceptionStatus.ProtocolError)

                return @"Summary:
             The response received from the server was complete but indicated a protocol-level
             error. For example, an HTTP protocol error such as 401 Access Denied would
             use this status.";
            if (state == WebExceptionStatus.ConnectionClosed)

                return @"Summary:
             The connection was prematurely closed.";
            if (state == WebExceptionStatus.TrustFailure)

                return @"Summary:
             A server certificate could not be validated.";
            if (state == WebExceptionStatus.SecureChannelFailure)

                return @"Summary:
             An error occurred while establishing a connection using SSL.";
            if (state == WebExceptionStatus.ServerProtocolViolation)

                return @"Summary:
             The server response was not a valid HTTP response.";
            if (state == WebExceptionStatus.KeepAliveFailure)

                return @"Summary:
             The connection for a request that specifies the Keep-alive header was closed
             unexpectedly.";
            if (state == WebExceptionStatus.Pending)

                return @"Summary:
             An internal asynchronous request is pending.";
            if (state == WebExceptionStatus.Timeout)
                return @"Summary:
             No response was received during the time-out period for a request.";
            if (state == WebExceptionStatus.ProxyNameResolutionFailure)

                return @"Summary:
             The name resolver service could not resolve the proxy host name.";
            if (state == WebExceptionStatus.UnknownError)

                return @"Summary:
             An exception of unknown type has occurred.";
            if (state == WebExceptionStatus.MessageLengthLimitExceeded)

                return @"Summary:
             A message was received that exceeded the specified limit when sending a request
             or receiving a response from the server.";
            if (state == WebExceptionStatus.CacheEntryNotFound)

                return @"Summary:
             The specified cache entry was not found.";
            if (state == WebExceptionStatus.RequestProhibitedByCachePolicy)

                return @"Summary:
             The request was not permitted by the cache policy. In general, this occurs
             when a request is not cacheable and the effective policy prohibits sending
             the request to the server. You might receive this status if a request method
             implies the presence of a request body, a request method requires direct
             interaction with the server, or a request contains a conditional header.";
            if (state == WebExceptionStatus.RequestProhibitedByProxy)

                return @"Summary:
             This request was not permitted by the proxy.";
            return "Unknown";
        }
        public static string GetErrorMessageByStatusCode(HttpStatusCode state)
        {
            if (state == HttpStatusCode.Continue)
                return @"Summary:
             Equivalent to HTTP status 100. Continue indicates
             that the client can continue with its request.";
            else if (state == HttpStatusCode.SwitchingProtocols)
                return @"Summary:
            Equivalent to HTTP status 101. SwitchingProtocols
            indicates that the protocol version or protocol is being changed.";
            else if (state == HttpStatusCode.OK)
                return @"Summary:
             Equivalent to HTTP status 200. OK indicates that
             the request succeeded and that the requested information is in the response.
             This is the most common status code to receive.";
            else if (state == HttpStatusCode.Created)

                return @"Summary:
             Equivalent to HTTP status 201. Created indicates
             that the request resulted in a new resource created before the response was
             sent.";
            else if (state == HttpStatusCode.Accepted)

                return @"Summary:
             Equivalent to HTTP status 202. Accepted indicates
             that the request has been accepted for further processing.";
            else if (state == HttpStatusCode.NonAuthoritativeInformation)

                return @"Summary:
             Equivalent to HTTP status 203. NonAuthoritativeInformation
             indicates that the returned metainformation is from a cached copy instead
             of the origin server and therefore may be incorrect.";
            else if (state == HttpStatusCode.NoContent)

                return @"Summary:
             Equivalent to HTTP status 204. NoContent indicates
             that the request has been successfully processed and that the response is
             intentionally blank.";
            else if (state == HttpStatusCode.ResetContent)
                return @"Summary:
             Equivalent to HTTP status 205. ResetContent indicates
             that the client should reset (not reload) the current resource.";
            else if (state == HttpStatusCode.PartialContent)

                return @"Summary:
             Equivalent to HTTP status 206. PartialContent indicates
             that the response is a partial response as requested by a GET request that
             includes a byte range.";
            else if (state == HttpStatusCode.MultipleChoices)

                return @"Summary:
             Equivalent to HTTP status 300. MultipleChoices
             indicates that the requested information has multiple representations. The
             default action is to treat this status as a redirect and follow the contents
             of the Location header associated with this response.";
            else if (state == HttpStatusCode.Ambiguous)

                return @"Summary:
             Equivalent to HTTP status 300. Ambiguous indicates
             that the requested information has multiple representations. The default
             action is to treat this status as a redirect and follow the contents of the
             Location header associated with this response.";
            else if (state == HttpStatusCode.MovedPermanently)

                return @"Summary:
             Equivalent to HTTP status 301. MovedPermanently
             indicates that the requested information has been moved to the URI specified
             in the Location header. The default action when this status is received is
             to follow the Location header associated with the response.";
            else if (state == HttpStatusCode.Moved)

                return @"Summary:
             Equivalent to HTTP status 301. Moved indicates
             that the requested information has been moved to the URI specified in the
             Location header. The default action when this status is received is to follow
             the Location header associated with the response. When the original request
             method was POST, the redirected request will use the GET method.";
            else if (state == HttpStatusCode.Found)

                return @"Summary:
             Equivalent to HTTP status 302. Found indicates
             that the requested information is located at the URI specified in the Location
             header. The default action when this status is received is to follow the
             Location header associated with the response. When the original request method
             was POST, the redirected request will use the GET method.";
            else if (state == HttpStatusCode.Redirect)

                return @"Summary:
             Equivalent to HTTP status 302. Redirect indicates
             that the requested information is located at the URI specified in the Location
             header. The default action when this status is received is to follow the
             Location header associated with the response. When the original request method
             was POST, the redirected request will use the GET method.";
            else if (state == HttpStatusCode.SeeOther)

                return @"Summary:
             Equivalent to HTTP status 303. SeeOther automatically
             redirects the client to the URI specified in the Location header as the result
             of a POST. The request to the resource specified by the Location header will
             be made with a GET.";
            else if (state == HttpStatusCode.RedirectMethod)

                return @"Summary:
             Equivalent to HTTP status 303. RedirectMethod automatically
             redirects the client to the URI specified in the Location header as the result
             of a POST. The request to the resource specified by the Location header will
             be made with a GET.";
            else if (state == HttpStatusCode.NotModified)

                return @"Summary:
             Equivalent to HTTP status 304. NotModified indicates
             that the client's cached copy is up to date. The contents of the resource
             are not transferred.";
            else if (state == HttpStatusCode.UseProxy)

                return @"Summary:
             Equivalent to HTTP status 305. UseProxy indicates
             that the request should use the proxy server at the URI specified in the
             Location header.";
            else if (state == HttpStatusCode.Unused)

                return @"Summary:
             Equivalent to HTTP status 306. Unused is a proposed
             extension to the HTTP/1.1 specification that is not fully specified.";
            else if (state == HttpStatusCode.TemporaryRedirect)

                return @"Summary:
             Equivalent to HTTP status 307. TemporaryRedirect
             indicates that the request information is located at the URI specified in
             the Location header. The default action when this status is received is to
             follow the Location header associated with the response. When the original
             request method was POST, the redirected request will also use the POST method.";
            else if (state == HttpStatusCode.RedirectKeepVerb)

                return @"Summary:
             Equivalent to HTTP status 307. RedirectKeepVerb
             indicates that the request information is located at the URI specified in
             the Location header. The default action when this status is received is to
             follow the Location header associated with the response. When the original
             request method was POST, the redirected request will also use the POST method.";
            else if (state == HttpStatusCode.BadRequest)

                return @"Summary:
             Equivalent to HTTP status 400. BadRequest indicates
             that the request could not be understood by the server. BadRequest
             is sent when no other error is applicable, or if the exact error is unknown
             or does not have its own error code.";
            else if (state == HttpStatusCode.Unauthorized)

                return @"Summary:
             Equivalent to HTTP status 401. Unauthorized indicates
             that the requested resource requires authentication. The WWW-Authenticate
             header contains the details of how to perform the authentication.";
            else if (state == HttpStatusCode.PaymentRequired)

                return @"Summary:
             Equivalent to HTTP status 402. PaymentRequired
             is reserved for future use.";
            else if (state == HttpStatusCode.Forbidden)

                return @"Summary:
             Equivalent to HTTP status 403. Forbidden indicates
             that the server refuses to fulfill the request.";
            else if (state == HttpStatusCode.NotFound)

                return @"Summary:
             Equivalent to HTTP status 404. NotFound indicates
             that the requested resource does not exist on the server.";
            else if (state == HttpStatusCode.MethodNotAllowed)

                return @"Summary:
             Equivalent to HTTP status 405. MethodNotAllowed
             indicates that the request method (POST or GET) is not allowed on the requested
             resource.";
            else if (state == HttpStatusCode.NotAcceptable)

                return @"Summary:
             Equivalent to HTTP status 406. NotAcceptable indicates
             that the client has indicated with Accept headers that it will not accept
             any of the available representations of the resource.";
            else if (state == HttpStatusCode.ProxyAuthenticationRequired)

                return @"Summary:
             Equivalent to HTTP status 407. ProxyAuthenticationRequired
             indicates that the requested proxy requires authentication. The Proxy-authenticate
             header contains the details of how to perform the authentication.";
            else if (state == HttpStatusCode.RequestTimeout)
                return @"Summary:
             Equivalent to HTTP status 408. RequestTimeout indicates
             that the client did not send a request within the time the server was expecting
             the request.";
            else if (state == HttpStatusCode.Conflict)

                return @"Summary:
             Equivalent to HTTP status 409. Conflict indicates
             that the request could not be carried out because of a conflict on the server.";
            else if (state == HttpStatusCode.Gone)

                return @"Summary:
             Equivalent to HTTP status 410. Gone indicates that
             the requested resource is no longer available.";
            else if (state == HttpStatusCode.LengthRequired)

                return @"Summary:
             Equivalent to HTTP status 411. LengthRequired indicates
             that the required Content-length header is missing.";
            else if (state == HttpStatusCode.PreconditionFailed)

                return @"Summary:
             Equivalent to HTTP status 412. PreconditionFailed
             indicates that a condition set for this request failed, and the request cannot
             be carried out. Conditions are set with conditional request headers like
             If-Match, If-None-Match, or If-Unmodified-Since.";
            else if (state == HttpStatusCode.RequestEntityTooLarge)

                return @"Summary:
             Equivalent to HTTP status 413. RequestEntityTooLarge
             indicates that the request is too large for the server to process.";
            else if (state == HttpStatusCode.RequestUriTooLong)

                return @"Summary:
             Equivalent to HTTP status 414. RequestUriTooLong
             indicates that the URI is too long.";
            else if (state == HttpStatusCode.UnsupportedMediaType)

                return @"Summary:
             Equivalent to HTTP status 415. UnsupportedMediaType
             indicates that the request is an unsupported type.";
            else if (state == HttpStatusCode.RequestedRangeNotSatisfiable)

                return @"Summary:
             Equivalent to HTTP status 416. RequestedRangeNotSatisfiable
             indicates that the range of data requested from the resource cannot be returned,
             either because the beginning of the range is before the beginning of the
             resource, or the end of the range is after the end of the resource.";
            else if (state == HttpStatusCode.ExpectationFailed)

                return @"Summary:
             Equivalent to HTTP status 417. ExpectationFailed
             indicates that an expectation given in an Expect header could not be met
             by the server.";
            else if (state == HttpStatusCode.InternalServerError)

                return @"Summary:
             Equivalent to HTTP status 500. InternalServerError
             indicates that a generic error has occurred on the server.";
            else if (state == HttpStatusCode.NotImplemented)

                return @"Summary:
             Equivalent to HTTP status 501. NotImplemented indicates
             that the server does not support the requested function.";
            else if (state == HttpStatusCode.BadGateway)
                return @"Summary:
             Equivalent to HTTP status 502. BadGateway indicates
             that an intermediate proxy server received a bad response from another proxy
             or the origin server.";
            else if (state == HttpStatusCode.ServiceUnavailable)

                return @"Summary:
             Equivalent to HTTP status 503. ServiceUnavailable
             indicates that the server is temporarily unavailable, usually due to high
             load or maintenance.";
            else if (state == HttpStatusCode.GatewayTimeout)

                return @"Summary:
             Equivalent to HTTP status 504. GatewayTimeout indicates
             that an intermediate proxy server timed out while waiting for a response
             from another proxy or the origin server.";
            else if (state == HttpStatusCode.HttpVersionNotSupported)

                return @"Summary:
             Equivalent to HTTP status 505. HttpVersionNotSupported
             indicates that the requested HTTP version is not supported by the server.
        HttpVersionNotSupported = 505,";
            return "Unknown";
        }
    }
}
