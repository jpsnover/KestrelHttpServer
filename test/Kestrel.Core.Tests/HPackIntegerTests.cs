// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Tests
{
    public class HPackIntegerTests
    {
        [Fact]
        public void IntegerEncoderDecoderRoundtrips()
        {
            var decoder = new IntegerDecoder();
            var range = 1 << 8;

            foreach (var i in Enumerable.Range(0, range).Concat(Enumerable.Range(int.MaxValue - range + 1, range)))
            {
                for (int n = 1; n <= 8; n++)
                {
                    var integerBytes = new byte[6];
                    Assert.True(IntegerEncoder.Encode(i, n, integerBytes, out var length));

                    var decodeResult = decoder.BeginDecode(integerBytes[0], n);

                    for (int j = 1; j < length; j++)
                    {
                        Assert.False(decodeResult);
                        decodeResult = decoder.Decode(integerBytes[j]);
                    }

                    Assert.True(decodeResult);
                    Assert.Equal(i, decoder.Value);
                }
            }
        }
    }
}
