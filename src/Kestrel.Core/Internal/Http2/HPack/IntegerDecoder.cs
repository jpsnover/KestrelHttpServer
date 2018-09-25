// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack
{
    public class IntegerDecoder
    {
        // The maximum we will decode is Int32.MaxValue, which is also the maximum request header field size.

        private int _i;
        private int _m;

        public int Value { get; private set; }

        public bool BeginDecode(byte b, int prefixLength)
        {
            if (b < ((1 << prefixLength) - 1))
            {
                Value = b;
                return true;
            }

            _i = b;
            _m = 0;
            return false;
        }

        public bool Decode(byte b)
        {
            _i += (b & 0x7f) << _m;
            _m += 7;

            if ((b & 0x80) != 0x80)
            {
                // Int32.MaxValue only needs a maximum of 5 bytes to represent and the last byte cannot have any value set larger than 0x7
                if ((_m > 28 && b > 0x7) || _i < 0)
                {
                    throw new HPackDecodingException("Integer too big");
                }

                Value = _i;
                return true;
            }
            else if (_m > 28)
            {
                // Int32.MaxValue only needs a maximum of 5 bytes to represent
                throw new HPackDecodingException("Integer too big");
            }

            return false;
        }
    }
}
