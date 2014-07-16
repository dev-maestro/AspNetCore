// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Infrastructure;
using Microsoft.AspNet.FeatureModel;
using Microsoft.AspNet.HttpFeature;
using Microsoft.AspNet.PipelineCore.Collections;
using Microsoft.AspNet.PipelineCore.Infrastructure;

namespace Microsoft.AspNet.PipelineCore
{
    public class RequestCookiesFeature : IRequestCookiesFeature
    {
        private readonly IFeatureCollection _features;
        private readonly FeatureReference<IHttpRequestFeature> _request = FeatureReference<IHttpRequestFeature>.Default;
        private string _cookiesHeader;
        private RequestCookiesCollection _cookiesCollection;
        private IReadableStringCollection _cookies;

        public RequestCookiesFeature([NotNull] IReadableStringCollection cookies)
        {
            _cookies = cookies;
        }

        public RequestCookiesFeature([NotNull] IFeatureCollection features)
        {
            _features = features;
        }

        public IReadableStringCollection Cookies
        {
            get
            {
                if (_features == null)
                {
                    return _cookies;
                }

                var headers = _request.Fetch(_features).Headers;
                string cookiesHeader = ParsingHelpers.GetHeader(headers, Constants.Headers.Cookie) ?? string.Empty;

                if (_cookiesCollection == null)
                {
                    _cookiesCollection = new RequestCookiesCollection();
                    _cookiesCollection.Reparse(cookiesHeader);
                    _cookiesHeader = cookiesHeader;
                }
                else if (!string.Equals(_cookiesHeader, cookiesHeader, StringComparison.Ordinal))
                {
                    _cookiesCollection.Reparse(cookiesHeader);
                    _cookiesHeader = cookiesHeader;
                }

                return _cookiesCollection;
            }
        }
    }
}