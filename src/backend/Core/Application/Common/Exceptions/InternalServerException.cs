﻿using System.Net;

namespace CodeMatrix.Mepd.Application.Common.Exceptions;

public class InternalServerException : CustomException
{
    public InternalServerException(string message, List<string> errors = default)
        : base(message, errors, HttpStatusCode.InternalServerError)
    {
    }
}
