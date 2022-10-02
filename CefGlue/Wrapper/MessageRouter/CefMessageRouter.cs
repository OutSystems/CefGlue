namespace Xilium.CefGlue.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // TODO: This implementation doesn't includes implementation for transfer
    // messages via shared memory region.
    // See CEF commit: 3dc94645838ad6c7f7821bce50015533da294d27 as reference
    // implementation.

    internal static class CefMessageRouter
    {
        // ID value reserved for internal use.
        public const int ReservedId = 0;

        // Appended to the JS function name for related IPC messages.
        public const string MessageSuffix = "Msg";

        // JS object member argument names for cefQuery.
        public const string MemberRequest = "request";
        public const string MemberOnSuccess = "onSuccess";
        public const string MemberOnFailure = "onFailure";
        public const string MemberPersistent = "persistent";

        // Default error information when a query is canceled.
        public const int CanceledErrorCode = -1;
        public const string CanceledErrorMessage = "The query has been canceled";

        // Value of 16KB is chosen as a result of performance tests available at
        // http://tests/ipc_performance
        public const int ResponseSizeThreshold = 16384;

        public sealed class IdGeneratorInt32
        {
            private int _next_id;

            public int GetNextId()
            {
                var id = ++_next_id;
                if (id == CefMessageRouter.ReservedId)
                    id = ++_next_id;
                return id;
            }
        }

        public sealed class IdGeneratorInt64
        {
            private long _next_id;

            public long GetNextId()
            {
                var id = ++_next_id;
                if (id == CefMessageRouter.ReservedId)
                    id = ++_next_id;
                return id;
            }
        }
    }
}
