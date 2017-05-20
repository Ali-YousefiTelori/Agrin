﻿namespace Agrin.LinkExtractor
{
    /// <summary>
    /// The video type. Also known as video container.
    /// </summary>
    public enum VideoType
    {
        /// <summary>
        /// Video for mobile devices (3GP).
        /// </summary>
        Mobile,

        Flash,
        Mp4,
        WebM,
        M4A,
        /// <summary>
        /// The video type is unknown. This can occur if YoutubeExtractor is not up-to-date.
        /// </summary>
        Unknown
    }
}