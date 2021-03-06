
using UnityEngine;
using System.Collections;

public class ReplaceFontFile : FileBase
{
	#region 定义
    const string FOT_KafuTechnoStd_H = "0039fdb1928583542b41b1e14ef9d517";
    const string FOT_CometStd_B = "de4dca105a7e7a747bee8eadf3d5f29e";
    const string FOT_NewRodinProN_DB = "15dc6c06b8ac24e438c271c02f761cef";
    const string FOT_RocknRollStd_DB = "a95a58c022cb88040a13667e48b6e0af";
    const string FOT_RodinNTLGPro = "d27fae711765d72408aa4ce23354ce23";
    const string Sawarabi_Gothic_Otf = "5d412c14cafc8a84e9112b1ae5b75b2c";

    private string _path = "";
    private string _filename = "";
    private string _encodeString = "";

    public ReplaceFontFile(string path, string filename)
    {
        _path = path;
        _filename = filename;
    }

	#endregion

	#region IO

    public void Replace()
    {
        if (Read(_path, _filename))
        {
            Write(_path, _filename);
        }  
    }

    protected override void Decode(string encodeString)
	{
        if (!string.IsNullOrEmpty(encodeString))
        {
            encodeString = encodeString.Replace(FOT_CometStd_B, FOT_KafuTechnoStd_H);
            encodeString = encodeString.Replace(FOT_NewRodinProN_DB, FOT_KafuTechnoStd_H);
            encodeString = encodeString.Replace(FOT_RocknRollStd_DB, FOT_KafuTechnoStd_H);
            encodeString = encodeString.Replace(FOT_RodinNTLGPro, FOT_KafuTechnoStd_H);
            encodeString = encodeString.Replace(Sawarabi_Gothic_Otf, FOT_KafuTechnoStd_H);
            _encodeString = encodeString;
        }  
	}

    protected override void Encode(out string encodeString)
	{
        encodeString = _encodeString;
	}

	#endregion
}
