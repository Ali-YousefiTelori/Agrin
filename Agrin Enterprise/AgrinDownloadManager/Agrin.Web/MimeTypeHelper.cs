using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Web
{
    public static class MimeTypeHelper
    {
        static string[] GetExtensions(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;
            if (typeName == "application/vnd.android.package-archive")
            {
                return new string[] { ".apk" };
            }
            else if (typeName == "x-world/x-3dmf")
            {
                return new string[] { ".3dm", ".3dmf", ".qd3", ".qd3d" };
            }
            else if (typeName == "application/octet-stream")
            {
                return new string[] { ".a", ".arc", ".arj", ".bin", ".com", ".dump", ".exe", ".lha", ".lhx", ".lzh", ".lzx", ".o", ".psd", ".saveme", ".uu", ".zoo", ".rar", ".zip" };
            }
            else if (typeName == "application/x-authorware-bin")
            {
                return new string[] { ".aab" };
            }
            else if (typeName == "application/x-authorware-map")
            {
                return new string[] { ".aam" };
            }
            else if (typeName == "application/x-authorware-seg")
            {
                return new string[] { ".aas" };
            }
            else if (typeName == "text/vnd.abc")
            {
                return new string[] { ".abc" };
            }
            else if (typeName == "text/html")
            {
                return new string[] { ".acgi", ".htm", ".html", ".htmls", ".htx", ".shtml" };
            }
            else if (typeName == "video/animaflex")
            {
                return new string[] { ".afl" };
            }
            else if (typeName == "application/postscript")
            {
                return new string[] { ".ai", ".eps", ".ps" };
            }
            else if (typeName == "audio/aiff")
            {
                return new string[] { ".aif", ".aifc", ".aiff" };
            }
            else if (typeName == "audio/x-aiff")
            {
                return new string[] { ".aif", ".aifc", ".aiff" };
            }
            else if (typeName == "application/x-aim")
            {
                return new string[] { ".aim" };
            }
            else if (typeName == "text/x-audiosoft-intra")
            {
                return new string[] { ".aip" };
            }
            else if (typeName == "application/x-navi-animation")
            {
                return new string[] { ".ani" };
            }
            else if (typeName == "application/x-nokia-9000-communicator-add-on-software")
            {
                return new string[] { ".aos" };
            }
            else if (typeName == "application/mime")
            {
                return new string[] { ".aps" };
            }
            else if (typeName == "application/arj")
            {
                return new string[] { ".arj" };
            }
            else if (typeName == "image/x-jg")
            {
                return new string[] { ".art" };
            }
            else if (typeName == "video/x-ms-asf")
            {
                return new string[] { ".asf", ".asx" };
            }
            else if (typeName == "text/x-asm")
            {
                return new string[] { ".asm", ".s" };
            }
            else if (typeName == "text/asp")
            {
                return new string[] { ".asp" };
            }
            else if (typeName == "application/x-mplayer2")
            {
                return new string[] { ".asx" };
            }
            else if (typeName == "video/x-ms-asf-plugin")
            {
                return new string[] { ".asx" };
            }
            else if (typeName == "audio/basic")
            {
                return new string[] { ".au", ".snd" };
            }
            else if (typeName == "audio/x-au")
            {
                return new string[] { ".au" };
            }
            else if (typeName == "application/x-troff-msvideo")
            {
                return new string[] { ".avi" };
            }
            else if (typeName == "video/avi")
            {
                return new string[] { ".avi" };
            }
            else if (typeName == "video/msvideo")
            {
                return new string[] { ".avi" };
            }
            else if (typeName == "video/x-msvideo")
            {
                return new string[] { ".avi" };
            }
            else if (typeName == "video/avs-video")
            {
                return new string[] { ".avs" };
            }
            else if (typeName == "application/x-bcpio")
            {
                return new string[] { ".bcpio" };
            }
            else if (typeName == "application/mac-binary")
            {
                return new string[] { ".bin" };
            }
            else if (typeName == "application/macbinary")
            {
                return new string[] { ".bin" };
            }
            else if (typeName == "application/x-binary")
            {
                return new string[] { ".bin" };
            }
            else if (typeName == "application/x-macbinary")
            {
                return new string[] { ".bin" };
            }
            else if (typeName == "image/bmp")
            {
                return new string[] { ".bm", ".bmp" };
            }
            else if (typeName == "image/x-windows-bmp")
            {
                return new string[] { ".bmp" };
            }
            else if (typeName == "application/book")
            {
                return new string[] { ".boo", ".book" };
            }
            else if (typeName == "application/x-bzip2")
            {
                return new string[] { ".boz", ".bz2" };
            }
            else if (typeName == "application/x-bsh")
            {
                return new string[] { ".bsh", ".sh", ".shar" };
            }
            else if (typeName == "application/x-bzip")
            {
                return new string[] { ".bz" };
            }
            else if (typeName == "text/plain")
            {
                return new string[] { ".c", ".c++", ".cc", ".com", ".conf", ".cxx", ".def", ".f", ".f90", ".for", ".g", ".h", ".hh", ".idc", ".jav", ".java", ".list", ".log", ".lst", ".m", ".mar", ".pl", ".sdml", ".text", ".txt" };
            }
            else if (typeName == "text/x-c")
            {
                return new string[] { ".c", ".cc", ".cpp" };
            }
            else if (typeName == "application/vnd.ms-pki.seccat")
            {
                return new string[] { ".cat" };
            }
            else if (typeName == "application/clariscad")
            {
                return new string[] { ".ccad" };
            }
            else if (typeName == "application/x-cocoa")
            {
                return new string[] { ".cco" };
            }
            else if (typeName == "application/cdf")
            {
                return new string[] { ".cdf" };
            }
            else if (typeName == "application/x-cdf")
            {
                return new string[] { ".cdf" };
            }
            else if (typeName == "application/x-netcdf")
            {
                return new string[] { ".cdf", ".nc" };
            }
            else if (typeName == "application/pkix-cert")
            {
                return new string[] { ".cer", ".crt" };
            }
            else if (typeName == "application/x-x509-ca-cert")
            {
                return new string[] { ".cer", ".crt", ".der" };
            }
            else if (typeName == "application/x-chat")
            {
                return new string[] { ".cha", ".chat" };
            }
            else if (typeName == "application/java")
            {
                return new string[] { ".class" };
            }
            else if (typeName == "application/java-byte-code")
            {
                return new string[] { ".class" };
            }
            else if (typeName == "application/x-java-class")
            {
                return new string[] { ".class" };
            }
            else if (typeName == "application/x-cpio")
            {
                return new string[] { ".cpio" };
            }
            else if (typeName == "application/mac-compactpro")
            {
                return new string[] { ".cpt" };
            }
            else if (typeName == "application/x-compactpro")
            {
                return new string[] { ".cpt" };
            }
            else if (typeName == "application/x-cpt")
            {
                return new string[] { ".cpt" };
            }
            else if (typeName == "application/pkcs-crl")
            {
                return new string[] { ".crl" };
            }
            else if (typeName == "application/pkix-crl")
            {
                return new string[] { ".crl" };
            }
            else if (typeName == "application/x-x509-user-cert")
            {
                return new string[] { ".crt" };
            }
            else if (typeName == "application/x-csh")
            {
                return new string[] { ".csh" };
            }
            else if (typeName == "text/x-script.csh")
            {
                return new string[] { ".csh" };
            }
            else if (typeName == "application/x-pointplus")
            {
                return new string[] { ".css" };
            }
            else if (typeName == "text/css")
            {
                return new string[] { ".css" };
            }
            else if (typeName == "application/x-director")
            {
                return new string[] { ".dcr", ".dir", ".dxr" };
            }
            else if (typeName == "application/x-deepv")
            {
                return new string[] { ".deepv" };
            }
            else if (typeName == "video/x-dv")
            {
                return new string[] { ".dif", ".dv" };
            }
            else if (typeName == "video/dl")
            {
                return new string[] { ".dl" };
            }
            else if (typeName == "video/x-dl")
            {
                return new string[] { ".dl" };
            }
            else if (typeName == "application/msword")
            {
                return new string[] { ".doc", ".dot", ".w6w", ".wiz", ".word" };
            }
            else if (typeName == "application/commonground")
            {
                return new string[] { ".dp" };
            }
            else if (typeName == "application/drafting")
            {
                return new string[] { ".drw" };
            }
            else if (typeName == "application/x-dvi")
            {
                return new string[] { ".dvi" };
            }
            else if (typeName == "drawing/x-dwf (old)")
            {
                return new string[] { ".dwf" };
            }
            else if (typeName == "model/vnd.dwf")
            {
                return new string[] { ".dwf" };
            }
            else if (typeName == "application/acad")
            {
                return new string[] { ".dwg" };
            }
            else if (typeName == "image/vnd.dwg")
            {
                return new string[] { ".dwg", ".dxf", ".svf" };
            }
            else if (typeName == "image/x-dwg")
            {
                return new string[] { ".dwg", ".dxf", ".svf" };
            }
            else if (typeName == "application/dxf")
            {
                return new string[] { ".dxf" };
            }
            else if (typeName == "text/x-script.elisp")
            {
                return new string[] { ".el" };
            }
            else if (typeName == "application/x-bytecode.elisp (compiled elisp)")
            {
                return new string[] { ".elc" };
            }
            else if (typeName == "application/x-elc")
            {
                return new string[] { ".elc" };
            }
            else if (typeName == "application/x-envoy")
            {
                return new string[] { ".env", ".evy" };
            }
            else if (typeName == "application/x-esrehber")
            {
                return new string[] { ".es" };
            }
            else if (typeName == "text/x-setext")
            {
                return new string[] { ".etx" };
            }
            else if (typeName == "application/envoy")
            {
                return new string[] { ".evy" };
            }
            else if (typeName == "text/x-fortran")
            {
                return new string[] { ".f", ".f77", ".f90", ".for" };
            }
            else if (typeName == "application/vnd.fdf")
            {
                return new string[] { ".fdf" };
            }
            else if (typeName == "application/fractals")
            {
                return new string[] { ".fif" };
            }
            else if (typeName == "image/fif")
            {
                return new string[] { ".fif" };
            }
            else if (typeName == "video/fli")
            {
                return new string[] { ".fli" };
            }
            else if (typeName == "video/x-fli")
            {
                return new string[] { ".fli" };
            }
            else if (typeName == "image/florian")
            {
                return new string[] { ".flo", ".turbot" };
            }
            else if (typeName == "text/vnd.fmi.flexstor")
            {
                return new string[] { ".flx" };
            }
            else if (typeName == "video/x-atomic3d-feature")
            {
                return new string[] { ".fmf" };
            }
            else if (typeName == "image/vnd.fpx")
            {
                return new string[] { ".fpx" };
            }
            else if (typeName == "image/vnd.net-fpx")
            {
                return new string[] { ".fpx" };
            }
            else if (typeName == "application/freeloader")
            {
                return new string[] { ".frl" };
            }
            else if (typeName == "audio/make")
            {
                return new string[] { ".funk", ".my", ".pfunk" };
            }
            else if (typeName == "image/g3fax")
            {
                return new string[] { ".g3" };
            }
            else if (typeName == "image/gif")
            {
                return new string[] { ".gif" };
            }
            else if (typeName == "video/gl")
            {
                return new string[] { ".gl" };
            }
            else if (typeName == "video/x-gl")
            {
                return new string[] { ".gl" };
            }
            else if (typeName == "audio/x-gsm")
            {
                return new string[] { ".gsd", ".gsm" };
            }
            else if (typeName == "application/x-gsp")
            {
                return new string[] { ".gsp" };
            }
            else if (typeName == "application/x-gss")
            {
                return new string[] { ".gss" };
            }
            else if (typeName == "application/x-gtar")
            {
                return new string[] { ".gtar" };
            }
            else if (typeName == "application/x-compressed")
            {
                return new string[] { ".gz", ".tgz", ".z", ".zip" };
            }
            else if (typeName == "application/x-gzip")
            {
                return new string[] { ".gz", ".gzip" };
            }
            else if (typeName == "multipart/x-gzip")
            {
                return new string[] { ".gzip" };
            }
            else if (typeName == "text/x-h")
            {
                return new string[] { ".h", ".hh" };
            }
            else if (typeName == "application/x-hdf")
            {
                return new string[] { ".hdf" };
            }
            else if (typeName == "application/x-helpfile")
            {
                return new string[] { ".help", ".hlp" };
            }
            else if (typeName == "application/vnd.hp-hpgl")
            {
                return new string[] { ".hgl", ".hpg", ".hpgl" };
            }
            else if (typeName == "text/x-script")
            {
                return new string[] { ".hlb" };
            }
            else if (typeName == "application/hlp")
            {
                return new string[] { ".hlp" };
            }
            else if (typeName == "application/x-winhelp")
            {
                return new string[] { ".hlp" };
            }
            else if (typeName == "application/binhex")
            {
                return new string[] { ".hqx" };
            }
            else if (typeName == "application/binhex4")
            {
                return new string[] { ".hqx" };
            }
            else if (typeName == "application/mac-binhex")
            {
                return new string[] { ".hqx" };
            }
            else if (typeName == "application/mac-binhex40")
            {
                return new string[] { ".hqx" };
            }
            else if (typeName == "application/x-binhex40")
            {
                return new string[] { ".hqx" };
            }
            else if (typeName == "application/x-mac-binhex40")
            {
                return new string[] { ".hqx" };
            }
            else if (typeName == "application/hta")
            {
                return new string[] { ".hta" };
            }
            else if (typeName == "text/x-component")
            {
                return new string[] { ".htc" };
            }
            else if (typeName == "text/webviewhtml")
            {
                return new string[] { ".htt" };
            }
            else if (typeName == "x-conference/x-cooltalk")
            {
                return new string[] { ".ice" };
            }
            else if (typeName == "image/x-icon")
            {
                return new string[] { ".ico" };
            }
            else if (typeName == "image/ief")
            {
                return new string[] { ".ief", ".iefs" };
            }
            else if (typeName == "application/iges")
            {
                return new string[] { ".iges", ".igs" };
            }
            else if (typeName == "model/iges")
            {
                return new string[] { ".iges", ".igs" };
            }
            else if (typeName == "application/x-ima")
            {
                return new string[] { ".ima" };
            }
            else if (typeName == "application/x-httpd-imap")
            {
                return new string[] { ".imap" };
            }
            else if (typeName == "application/inf")
            {
                return new string[] { ".inf" };
            }
            else if (typeName == "application/x-internett-signup")
            {
                return new string[] { ".ins" };
            }
            else if (typeName == "application/x-ip2")
            {
                return new string[] { ".ip" };
            }
            else if (typeName == "video/x-isvideo")
            {
                return new string[] { ".isu" };
            }
            else if (typeName == "audio/it")
            {
                return new string[] { ".it" };
            }
            else if (typeName == "application/x-inventor")
            {
                return new string[] { ".iv" };
            }
            else if (typeName == "i-world/i-vrml")
            {
                return new string[] { ".ivr" };
            }
            else if (typeName == "application/x-livescreen")
            {
                return new string[] { ".ivy" };
            }
            else if (typeName == "audio/x-jam")
            {
                return new string[] { ".jam" };
            }
            else if (typeName == "text/x-java-source")
            {
                return new string[] { ".jav", ".java" };
            }
            else if (typeName == "application/x-java-commerce")
            {
                return new string[] { ".jcm" };
            }
            else if (typeName == "image/jpeg")
            {
                return new string[] { ".jfif", ".jfif-tbnl", ".jpe", ".jpeg", ".jpg" };
            }
            else if (typeName == "image/pjpeg")
            {
                return new string[] { ".jfif", ".jpe", ".jpeg", ".jpg" };
            }
            else if (typeName == "image/x-jps")
            {
                return new string[] { ".jps" };
            }
            else if (typeName == "application/x-javascript")
            {
                return new string[] { ".js" };
            }
            else if (typeName == "application/javascript")
            {
                return new string[] { ".js" };
            }
            else if (typeName == "application/ecmascript")
            {
                return new string[] { ".js" };
            }
            else if (typeName == "text/javascript")
            {
                return new string[] { ".js" };
            }
            else if (typeName == "text/ecmascript")
            {
                return new string[] { ".js" };
            }
            else if (typeName == "image/jutvision")
            {
                return new string[] { ".jut" };
            }
            else if (typeName == "audio/midi")
            {
                return new string[] { ".kar", ".mid", ".midi" };
            }
            else if (typeName == "music/x-karaoke")
            {
                return new string[] { ".kar" };
            }
            else if (typeName == "application/x-ksh")
            {
                return new string[] { ".ksh" };
            }
            else if (typeName == "text/x-script.ksh")
            {
                return new string[] { ".ksh" };
            }
            else if (typeName == "audio/nspaudio")
            {
                return new string[] { ".la", ".lma" };
            }
            else if (typeName == "audio/x-nspaudio")
            {
                return new string[] { ".la", ".lma" };
            }
            else if (typeName == "audio/x-liveaudio")
            {
                return new string[] { ".lam" };
            }
            else if (typeName == "application/x-latex")
            {
                return new string[] { ".latex", ".ltx" };
            }
            else if (typeName == "application/lha")
            {
                return new string[] { ".lha" };
            }
            else if (typeName == "application/x-lha")
            {
                return new string[] { ".lha" };
            }
            else if (typeName == "application/x-lisp")
            {
                return new string[] { ".lsp" };
            }
            else if (typeName == "text/x-script.lisp")
            {
                return new string[] { ".lsp" };
            }
            else if (typeName == "text/x-la-asf")
            {
                return new string[] { ".lsx" };
            }
            else if (typeName == "application/x-lzh")
            {
                return new string[] { ".lzh" };
            }
            else if (typeName == "application/lzx")
            {
                return new string[] { ".lzx" };
            }
            else if (typeName == "application/x-lzx")
            {
                return new string[] { ".lzx" };
            }
            else if (typeName == "text/x-m")
            {
                return new string[] { ".m" };
            }
            else if (typeName == "video/mpeg")
            {
                return new string[] { ".m1v", ".m2v", ".mp2", ".mp3", ".mpa", ".mpe", ".mpeg", ".mpg" };
            }
            else if (typeName == "audio/mpeg")
            {
                return new string[] { ".m2a", ".mp2", ".mpa", ".mpg", ".mpga" };
            }
            else if (typeName == "audio/x-mpequrl")
            {
                return new string[] { ".m3u" };
            }
            else if (typeName == "application/x-troff-man")
            {
                return new string[] { ".man" };
            }
            else if (typeName == "application/x-navimap")
            {
                return new string[] { ".map" };
            }
            else if (typeName == "application/mbedlet")
            {
                return new string[] { ".mbd" };
            }
            else if (typeName == "application/x-magic-cap-package-1.0")
            {
                return new string[] { ".mc$" };
            }
            else if (typeName == "application/mcad")
            {
                return new string[] { ".mcd" };
            }
            else if (typeName == "application/x-mathcad")
            {
                return new string[] { ".mcd" };
            }
            else if (typeName == "image/vasa")
            {
                return new string[] { ".mcf" };
            }
            else if (typeName == "text/mcf")
            {
                return new string[] { ".mcf" };
            }
            else if (typeName == "application/netmc")
            {
                return new string[] { ".mcp" };
            }
            else if (typeName == "application/x-troff-me")
            {
                return new string[] { ".me" };
            }
            else if (typeName == "message/rfc822")
            {
                return new string[] { ".mht", ".mhtml", ".mime" };
            }
            else if (typeName == "application/x-midi")
            {
                return new string[] { ".mid", ".midi" };
            }
            else if (typeName == "audio/x-mid")
            {
                return new string[] { ".mid", ".midi" };
            }
            else if (typeName == "audio/x-midi")
            {
                return new string[] { ".mid", ".midi" };
            }
            else if (typeName == "music/crescendo")
            {
                return new string[] { ".mid", ".midi" };
            }
            else if (typeName == "x-music/x-midi")
            {
                return new string[] { ".mid", ".midi" };
            }
            else if (typeName == "application/x-frame")
            {
                return new string[] { ".mif" };
            }
            else if (typeName == "application/x-mif")
            {
                return new string[] { ".mif" };
            }
            else if (typeName == "www/mime")
            {
                return new string[] { ".mime" };
            }
            else if (typeName == "audio/x-vnd.audioexplosion.mjuicemediafile")
            {
                return new string[] { ".mjf" };
            }
            else if (typeName == "video/x-motion-jpeg")
            {
                return new string[] { ".mjpg" };
            }
            else if (typeName == "application/base64")
            {
                return new string[] { ".mm", ".mme" };
            }
            else if (typeName == "application/x-meme")
            {
                return new string[] { ".mm" };
            }
            else if (typeName == "audio/mod")
            {
                return new string[] { ".mod" };
            }
            else if (typeName == "audio/x-mod")
            {
                return new string[] { ".mod" };
            }
            else if (typeName == "video/quicktime")
            {
                return new string[] { ".moov", ".mov", ".qt" };
            }
            else if (typeName == "video/x-sgi-movie")
            {
                return new string[] { ".movie", ".mv" };
            }
            else if (typeName == "audio/x-mpeg")
            {
                return new string[] { ".mp2" };
            }
            else if (typeName == "video/x-mpeg")
            {
                return new string[] { ".mp2", ".mp3" };
            }
            else if (typeName == "video/x-mpeq2a")
            {
                return new string[] { ".mp2" };
            }
            else if (typeName == "audio/mpeg3")
            {
                return new string[] { ".mp3" };
            }
            else if (typeName == "audio/x-mpeg-3")
            {
                return new string[] { ".mp3" };
            }
            else if (typeName == "application/x-project")
            {
                return new string[] { ".mpc", ".mpt", ".mpv", ".mpx" };
            }
            else if (typeName == "application/vnd.ms-project")
            {
                return new string[] { ".mpp" };
            }
            else if (typeName == "application/marc")
            {
                return new string[] { ".mrc" };
            }
            else if (typeName == "application/x-troff-ms")
            {
                return new string[] { ".ms" };
            }
            else if (typeName == "application/x-vnd.audioexplosion.mzz")
            {
                return new string[] { ".mzz" };
            }
            else if (typeName == "image/naplps")
            {
                return new string[] { ".nap", ".naplps" };
            }
            else if (typeName == "application/vnd.nokia.configuration-message")
            {
                return new string[] { ".ncm" };
            }
            else if (typeName == "image/x-niff")
            {
                return new string[] { ".nif", ".niff" };
            }
            else if (typeName == "application/x-mix-transfer")
            {
                return new string[] { ".nix" };
            }
            else if (typeName == "application/x-conference")
            {
                return new string[] { ".nsc" };
            }
            else if (typeName == "application/x-navidoc")
            {
                return new string[] { ".nvd" };
            }
            else if (typeName == "application/oda")
            {
                return new string[] { ".oda" };
            }
            else if (typeName == "application/x-omc")
            {
                return new string[] { ".omc" };
            }
            else if (typeName == "application/x-omcdatamaker")
            {
                return new string[] { ".omcd" };
            }
            else if (typeName == "application/x-omcregerator")
            {
                return new string[] { ".omcr" };
            }
            else if (typeName == "text/x-pascal")
            {
                return new string[] { ".p" };
            }
            else if (typeName == "application/pkcs10")
            {
                return new string[] { ".p10" };
            }
            else if (typeName == "application/x-pkcs10")
            {
                return new string[] { ".p10" };
            }
            else if (typeName == "application/pkcs-12")
            {
                return new string[] { ".p12" };
            }
            else if (typeName == "application/x-pkcs12")
            {
                return new string[] { ".p12" };
            }
            else if (typeName == "application/x-pkcs7-signature")
            {
                return new string[] { ".p7a" };
            }
            else if (typeName == "application/pkcs7-mime")
            {
                return new string[] { ".p7c", ".p7m" };
            }
            else if (typeName == "application/x-pkcs7-mime")
            {
                return new string[] { ".p7c", ".p7m" };
            }
            else if (typeName == "application/x-pkcs7-certreqresp")
            {
                return new string[] { ".p7r" };
            }
            else if (typeName == "application/pkcs7-signature")
            {
                return new string[] { ".p7s" };
            }
            else if (typeName == "application/pro_eng")
            {
                return new string[] { ".part", ".prt" };
            }
            else if (typeName == "text/pascal")
            {
                return new string[] { ".pas" };
            }
            else if (typeName == "image/x-portable-bitmap")
            {
                return new string[] { ".pbm" };
            }
            else if (typeName == "application/vnd.hp-pcl")
            {
                return new string[] { ".pcl" };
            }
            else if (typeName == "application/x-pcl")
            {
                return new string[] { ".pcl" };
            }
            else if (typeName == "image/x-pict")
            {
                return new string[] { ".pct" };
            }
            else if (typeName == "image/x-pcx")
            {
                return new string[] { ".pcx" };
            }
            else if (typeName == "chemical/x-pdb")
            {
                return new string[] { ".pdb", ".xyz" };
            }
            else if (typeName == "application/pdf")
            {
                return new string[] { ".pdf" };
            }
            else if (typeName == "audio/make.my.funk")
            {
                return new string[] { ".pfunk" };
            }
            else if (typeName == "image/x-portable-graymap")
            {
                return new string[] { ".pgm" };
            }
            else if (typeName == "image/x-portable-greymap")
            {
                return new string[] { ".pgm" };
            }
            else if (typeName == "image/pict")
            {
                return new string[] { ".pic", ".pict" };
            }
            else if (typeName == "application/x-newton-compatible-pkg")
            {
                return new string[] { ".pkg" };
            }
            else if (typeName == "application/vnd.ms-pki.pko")
            {
                return new string[] { ".pko" };
            }
            else if (typeName == "text/x-script.perl")
            {
                return new string[] { ".pl" };
            }
            else if (typeName == "application/x-pixclscript")
            {
                return new string[] { ".plx" };
            }
            else if (typeName == "image/x-xpixmap")
            {
                return new string[] { ".pm", ".xpm" };
            }
            else if (typeName == "text/x-script.perl-module")
            {
                return new string[] { ".pm" };
            }
            else if (typeName == "application/x-pagemaker")
            {
                return new string[] { ".pm4", ".pm5" };
            }
            else if (typeName == "image/png")
            {
                return new string[] { ".png", ".x-png" };
            }
            else if (typeName == "application/x-portable-anymap")
            {
                return new string[] { ".pnm" };
            }
            else if (typeName == "image/x-portable-anymap")
            {
                return new string[] { ".pnm" };
            }
            else if (typeName == "application/mspowerpoint")
            {
                return new string[] { ".pot", ".pps", ".ppt", ".ppz" };
            }
            else if (typeName == "application/vnd.ms-powerpoint")
            {
                return new string[] { ".pot", ".ppa", ".pps", ".ppt", ".pwz" };
            }
            else if (typeName == "model/x-pov")
            {
                return new string[] { ".pov" };
            }
            else if (typeName == "image/x-portable-pixmap")
            {
                return new string[] { ".ppm" };
            }
            else if (typeName == "application/powerpoint")
            {
                return new string[] { ".ppt" };
            }
            else if (typeName == "application/x-mspowerpoint")
            {
                return new string[] { ".ppt" };
            }
            else if (typeName == "application/x-freelance")
            {
                return new string[] { ".pre" };
            }
            else if (typeName == "paleovu/x-pv")
            {
                return new string[] { ".pvu" };
            }
            else if (typeName == "text/x-script.phyton")
            {
                return new string[] { ".py" };
            }
            else if (typeName == "application/x-bytecode.python")
            {
                return new string[] { ".pyc" };
            }
            else if (typeName == "audio/vnd.qcelp")
            {
                return new string[] { ".qcp" };
            }
            else if (typeName == "image/x-quicktime")
            {
                return new string[] { ".qif", ".qti", ".qtif" };
            }
            else if (typeName == "video/x-qtc")
            {
                return new string[] { ".qtc" };
            }
            else if (typeName == "audio/x-pn-realaudio")
            {
                return new string[] { ".ra", ".ram", ".rm", ".rmm", ".rmp" };
            }
            else if (typeName == "audio/x-pn-realaudio-plugin")
            {
                return new string[] { ".ra", ".rmp", ".rpm" };
            }
            else if (typeName == "audio/x-realaudio")
            {
                return new string[] { ".ra" };
            }
            else if (typeName == "application/x-cmu-raster")
            {
                return new string[] { ".ras" };
            }
            else if (typeName == "image/cmu-raster")
            {
                return new string[] { ".ras", ".rast" };
            }
            else if (typeName == "image/x-cmu-raster")
            {
                return new string[] { ".ras" };
            }
            else if (typeName == "text/x-script.rexx")
            {
                return new string[] { ".rexx" };
            }
            else if (typeName == "image/vnd.rn-realflash")
            {
                return new string[] { ".rf" };
            }
            else if (typeName == "image/x-rgb")
            {
                return new string[] { ".rgb" };
            }
            else if (typeName == "application/vnd.rn-realmedia")
            {
                return new string[] { ".rm" };
            }
            else if (typeName == "audio/mid")
            {
                return new string[] { ".rmi" };
            }
            else if (typeName == "application/ringing-tones")
            {
                return new string[] { ".rng" };
            }
            else if (typeName == "application/vnd.nokia.ringing-tone")
            {
                return new string[] { ".rng" };
            }
            else if (typeName == "application/vnd.rn-realplayer")
            {
                return new string[] { ".rnx" };
            }
            else if (typeName == "application/x-troff")
            {
                return new string[] { ".roff", ".t", ".tr" };
            }
            else if (typeName == "image/vnd.rn-realpix")
            {
                return new string[] { ".rp" };
            }
            else if (typeName == "text/richtext")
            {
                return new string[] { ".rt", ".rtf", ".rtx" };
            }
            else if (typeName == "text/vnd.rn-realtext")
            {
                return new string[] { ".rt" };
            }
            else if (typeName == "application/rtf")
            {
                return new string[] { ".rtf", ".rtx" };
            }
            else if (typeName == "application/x-rtf")
            {
                return new string[] { ".rtf" };
            }
            else if (typeName == "video/vnd.rn-realvideo")
            {
                return new string[] { ".rv" };
            }
            else if (typeName == "audio/s3m")
            {
                return new string[] { ".s3m" };
            }
            else if (typeName == "application/x-tbook")
            {
                return new string[] { ".sbk", ".tbk" };
            }
            else if (typeName == "application/x-lotusscreencam")
            {
                return new string[] { ".scm" };
            }
            else if (typeName == "text/x-script.guile")
            {
                return new string[] { ".scm" };
            }
            else if (typeName == "text/x-script.scheme")
            {
                return new string[] { ".scm" };
            }
            else if (typeName == "video/x-scm")
            {
                return new string[] { ".scm" };
            }
            else if (typeName == "application/sdp")
            {
                return new string[] { ".sdp" };
            }
            else if (typeName == "application/x-sdp")
            {
                return new string[] { ".sdp" };
            }
            else if (typeName == "application/sounder")
            {
                return new string[] { ".sdr" };
            }
            else if (typeName == "application/sea")
            {
                return new string[] { ".sea" };
            }
            else if (typeName == "application/x-sea")
            {
                return new string[] { ".sea" };
            }
            else if (typeName == "application/set")
            {
                return new string[] { ".set" };
            }
            else if (typeName == "text/sgml")
            {
                return new string[] { ".sgm", ".sgml" };
            }
            else if (typeName == "text/x-sgml")
            {
                return new string[] { ".sgm", ".sgml" };
            }
            else if (typeName == "application/x-sh")
            {
                return new string[] { ".sh" };
            }
            else if (typeName == "application/x-shar")
            {
                return new string[] { ".sh", ".shar" };
            }
            else if (typeName == "text/x-script.sh")
            {
                return new string[] { ".sh" };
            }
            else if (typeName == "text/x-server-parsed-html")
            {
                return new string[] { ".shtml", ".ssi" };
            }
            else if (typeName == "audio/x-psid")
            {
                return new string[] { ".sid" };
            }
            else if (typeName == "application/x-sit")
            {
                return new string[] { ".sit" };
            }
            else if (typeName == "application/x-stuffit")
            {
                return new string[] { ".sit" };
            }
            else if (typeName == "application/x-koan")
            {
                return new string[] { ".skd", ".skm", ".skp", ".skt" };
            }
            else if (typeName == "application/x-seelogo")
            {
                return new string[] { ".sl" };
            }
            else if (typeName == "application/smil")
            {
                return new string[] { ".smi", ".smil" };
            }
            else if (typeName == "audio/x-adpcm")
            {
                return new string[] { ".snd" };
            }
            else if (typeName == "application/solids")
            {
                return new string[] { ".sol" };
            }
            else if (typeName == "application/x-pkcs7-certificates")
            {
                return new string[] { ".spc" };
            }
            else if (typeName == "text/x-speech")
            {
                return new string[] { ".spc", ".talk" };
            }
            else if (typeName == "application/futuresplash")
            {
                return new string[] { ".spl" };
            }
            else if (typeName == "application/x-sprite")
            {
                return new string[] { ".spr", ".sprite" };
            }
            else if (typeName == "application/x-wais-source")
            {
                return new string[] { ".src", ".wsrc" };
            }
            else if (typeName == "application/streamingmedia")
            {
                return new string[] { ".ssm" };
            }
            else if (typeName == "application/vnd.ms-pki.certstore")
            {
                return new string[] { ".sst" };
            }
            else if (typeName == "application/step")
            {
                return new string[] { ".step", ".stp" };
            }
            else if (typeName == "application/sla")
            {
                return new string[] { ".stl" };
            }
            else if (typeName == "application/vnd.ms-pki.stl")
            {
                return new string[] { ".stl" };
            }
            else if (typeName == "application/x-navistyle")
            {
                return new string[] { ".stl" };
            }
            else if (typeName == "application/x-sv4cpio")
            {
                return new string[] { ".sv4cpio" };
            }
            else if (typeName == "application/x-sv4crc")
            {
                return new string[] { ".sv4crc" };
            }
            else if (typeName == "application/x-world")
            {
                return new string[] { ".svr", ".wrl" };
            }
            else if (typeName == "x-world/x-svr")
            {
                return new string[] { ".svr" };
            }
            else if (typeName == "application/x-shockwave-flash")
            {
                return new string[] { ".swf" };
            }
            else if (typeName == "application/x-tar")
            {
                return new string[] { ".tar" };
            }
            else if (typeName == "application/toolbook")
            {
                return new string[] { ".tbk" };
            }
            else if (typeName == "application/x-tcl")
            {
                return new string[] { ".tcl" };
            }
            else if (typeName == "text/x-script.tcl")
            {
                return new string[] { ".tcl" };
            }
            else if (typeName == "text/x-script.tcsh")
            {
                return new string[] { ".tcsh" };
            }
            else if (typeName == "application/x-tex")
            {
                return new string[] { ".tex" };
            }
            else if (typeName == "application/x-texinfo")
            {
                return new string[] { ".texi", ".texinfo" };
            }
            else if (typeName == "application/plain")
            {
                return new string[] { ".text" };
            }
            else if (typeName == "application/gnutar")
            {
                return new string[] { ".tgz" };
            }
            else if (typeName == "image/tiff")
            {
                return new string[] { ".tif", ".tiff" };
            }
            else if (typeName == "image/x-tiff")
            {
                return new string[] { ".tif", ".tiff" };
            }
            else if (typeName == "audio/tsp-audio")
            {
                return new string[] { ".tsi" };
            }
            else if (typeName == "application/dsptype")
            {
                return new string[] { ".tsp" };
            }
            else if (typeName == "audio/tsplayer")
            {
                return new string[] { ".tsp" };
            }
            else if (typeName == "text/tab-separated-values")
            {
                return new string[] { ".tsv" };
            }
            else if (typeName == "text/x-uil")
            {
                return new string[] { ".uil" };
            }
            else if (typeName == "text/uri-list")
            {
                return new string[] { ".uni", ".unis", ".uri", ".uris" };
            }
            else if (typeName == "application/i-deas")
            {
                return new string[] { ".unv" };
            }
            else if (typeName == "application/x-ustar")
            {
                return new string[] { ".ustar" };
            }
            else if (typeName == "multipart/x-ustar")
            {
                return new string[] { ".ustar" };
            }
            else if (typeName == "text/x-uuencode")
            {
                return new string[] { ".uu", ".uue" };
            }
            else if (typeName == "application/x-cdlink")
            {
                return new string[] { ".vcd" };
            }
            else if (typeName == "text/x-vcalendar")
            {
                return new string[] { ".vcs" };
            }
            else if (typeName == "application/vda")
            {
                return new string[] { ".vda" };
            }
            else if (typeName == "video/vdo")
            {
                return new string[] { ".vdo" };
            }
            else if (typeName == "application/groupwise")
            {
                return new string[] { ".vew" };
            }
            else if (typeName == "video/vivo")
            {
                return new string[] { ".viv", ".vivo" };
            }
            else if (typeName == "video/vnd.vivo")
            {
                return new string[] { ".viv", ".vivo" };
            }
            else if (typeName == "application/vocaltec-media-desc")
            {
                return new string[] { ".vmd" };
            }
            else if (typeName == "application/vocaltec-media-file")
            {
                return new string[] { ".vmf" };
            }
            else if (typeName == "audio/voc")
            {
                return new string[] { ".voc" };
            }
            else if (typeName == "audio/x-voc")
            {
                return new string[] { ".voc" };
            }
            else if (typeName == "video/vosaic")
            {
                return new string[] { ".vos" };
            }
            else if (typeName == "audio/voxware")
            {
                return new string[] { ".vox" };
            }
            else if (typeName == "audio/x-twinvq-plugin")
            {
                return new string[] { ".vqe", ".vql" };
            }
            else if (typeName == "audio/x-twinvq")
            {
                return new string[] { ".vqf" };
            }
            else if (typeName == "application/x-vrml")
            {
                return new string[] { ".vrml" };
            }
            else if (typeName == "model/vrml")
            {
                return new string[] { ".vrml", ".wrl", ".wrz" };
            }
            else if (typeName == "x-world/x-vrml")
            {
                return new string[] { ".vrml", ".wrl", ".wrz" };
            }
            else if (typeName == "x-world/x-vrt")
            {
                return new string[] { ".vrt" };
            }
            else if (typeName == "application/x-visio")
            {
                return new string[] { ".vsd", ".vst", ".vsw" };
            }
            else if (typeName == "application/wordperfect6.0")
            {
                return new string[] { ".w60", ".wp5" };
            }
            else if (typeName == "application/wordperfect6.1")
            {
                return new string[] { ".w61" };
            }
            else if (typeName == "audio/wav")
            {
                return new string[] { ".wav" };
            }
            else if (typeName == "audio/x-wav")
            {
                return new string[] { ".wav" };
            }
            else if (typeName == "application/x-qpro")
            {
                return new string[] { ".wb1" };
            }
            else if (typeName == "image/vnd.wap.wbmp")
            {
                return new string[] { ".wbmp" };
            }
            else if (typeName == "application/vnd.xara")
            {
                return new string[] { ".web" };
            }
            else if (typeName == "application/x-123")
            {
                return new string[] { ".wk1" };
            }
            else if (typeName == "windows/metafile")
            {
                return new string[] { ".wmf" };
            }
            else if (typeName == "text/vnd.wap.wml")
            {
                return new string[] { ".wml" };
            }
            else if (typeName == "application/vnd.wap.wmlc")
            {
                return new string[] { ".wmlc" };
            }
            else if (typeName == "text/vnd.wap.wmlscript")
            {
                return new string[] { ".wmls" };
            }
            else if (typeName == "application/vnd.wap.wmlscriptc")
            {
                return new string[] { ".wmlsc" };
            }
            else if (typeName == "application/wordperfect")
            {
                return new string[] { ".wp", ".wp5", ".wp6", ".wpd" };
            }
            else if (typeName == "application/x-wpwin")
            {
                return new string[] { ".wpd" };
            }
            else if (typeName == "application/x-lotus")
            {
                return new string[] { ".wq1" };
            }
            else if (typeName == "application/mswrite")
            {
                return new string[] { ".wri" };
            }
            else if (typeName == "application/x-wri")
            {
                return new string[] { ".wri" };
            }
            else if (typeName == "text/scriplet")
            {
                return new string[] { ".wsc" };
            }
            else if (typeName == "application/x-wintalk")
            {
                return new string[] { ".wtk" };
            }
            else if (typeName == "image/x-xbitmap")
            {
                return new string[] { ".xbm" };
            }
            else if (typeName == "image/x-xbm")
            {
                return new string[] { ".xbm" };
            }
            else if (typeName == "image/xbm")
            {
                return new string[] { ".xbm" };
            }
            else if (typeName == "video/x-amt-demorun")
            {
                return new string[] { ".xdr" };
            }
            else if (typeName == "xgl/drawing")
            {
                return new string[] { ".xgz" };
            }
            else if (typeName == "image/vnd.xiff")
            {
                return new string[] { ".xif" };
            }
            else if (typeName == "application/excel")
            {
                return new string[] { ".xl", ".xla", ".xlb", ".xlc", ".xld", ".xlk", ".xll", ".xlm", ".xls", ".xlt", ".xlv", ".xlw" };
            }
            else if (typeName == "application/x-excel")
            {
                return new string[] { ".xla", ".xlb", ".xlc", ".xld", ".xlk", ".xll", ".xlm", ".xls", ".xlt", ".xlv", ".xlw" };
            }
            else if (typeName == "application/x-msexcel")
            {
                return new string[] { ".xla", ".xls", ".xlw" };
            }
            else if (typeName == "application/vnd.ms-excel")
            {
                return new string[] { ".xlb", ".xlc", ".xll", ".xlm", ".xls", ".xlw" };
            }
            else if (typeName == "audio/xm")
            {
                return new string[] { ".xm" };
            }
            else if (typeName == "application/xml")
            {
                return new string[] { ".xml" };
            }
            else if (typeName == "text/xml")
            {
                return new string[] { ".xml" };
            }
            else if (typeName == "xgl/movie")
            {
                return new string[] { ".xmz" };
            }
            else if (typeName == "application/x-vnd.ls-xpix")
            {
                return new string[] { ".xpix" };
            }
            else if (typeName == "image/xpm")
            {
                return new string[] { ".xpm" };
            }
            else if (typeName == "video/x-amt-showrun")
            {
                return new string[] { ".xsr" };
            }
            else if (typeName == "image/x-xwd")
            {
                return new string[] { ".xwd" };
            }
            else if (typeName == "image/x-xwindowdump")
            {
                return new string[] { ".xwd" };
            }
            else if (typeName == "application/x-compress")
            {
                return new string[] { ".z" };
            }
            else if (typeName == "application/x-zip-compressed")
            {
                return new string[] { ".zip" };
            }
            else if (typeName == "application/zip")
            {
                return new string[] { ".zip" };
            }
            else if (typeName == "multipart/x-zip")
            {
                return new string[] { ".zip" };
            }
            else if (typeName == "text/x-script.zsh")
            {
                return new string[] { ".zsh" };
            }
            return null;
        }
        public static string GetExtension(string typeName)
        {
            var items = GetExtensions(typeName.ToLower());
            if (items == null)
            {
                AutoLogger.LogText($"MimeType not found!{typeName}");
                return null;
            }
            return items.FirstOrDefault();
        }
    }
}
