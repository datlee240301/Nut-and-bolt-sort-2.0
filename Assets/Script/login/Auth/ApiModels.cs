using System;

namespace ScrewJam.Auth
{
    // -------- Login --------
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginResponse
    {
        public string _id;
        public string username;
        public string role;
        public string token;
    }

    // -------- Verify Purchase --------
    // NOTE: receipt is sent as a raw JSON object. To make it work with
    // Unity's JsonUtility (which doesn't support arbitrary object types),
    // VerifyPurchaseRequest is serialized manually inside ApiService.
    [Serializable]
    public class VerifyPurchaseResponse
    {
        public string message;
    }

    // Generic server error wrapper. Many REST backends return { "message": "..." }
    // for errors as well, so we reuse the same shape for both success and error parsing.
    [Serializable]
    public class ApiErrorResponse
    {
        public string message;
    }
}
