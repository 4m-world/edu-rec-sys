using System.ComponentModel;

namespace CodeMatrix.Mepd.Shared.DTOs.Common;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")]
    Image,

    [Description(".xls,.xlsx,.csv,.json")]
    DataFile,

    [Description(".pdf,.doc,.docx")]
    Document
}