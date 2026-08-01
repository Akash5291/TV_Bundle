using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class Utility
{

	public static float PixelToMeterValue = 0.0002645833f;

	public static bool CheckIsUrlFormat(string strValue)
	{
		return Utility.CheckIsFormat("(http://)?([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?", strValue);
	}

	public static bool CheckIsFormat(string strRegex, string strValue)
	{
		if (strValue != null && strValue.Trim() != string.Empty)
		{
			Regex regex = new Regex(strRegex);
			return regex.IsMatch(strValue);
		}
		return false;
	}



	//Converter

	public static Sprite Texture2DToSprite(Texture2D texture)
	{
		var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
		return sprite;
	}

	  public static Vector2 PixelToMeter(float w,float h) {
            Vector2 data = new Vector2((w * PixelToMeterValue), (h * PixelToMeterValue));
            return data;
        }


	public static string HTTPStatusError(long code) {

		string message = "";

        switch (code)
        {

            case 201:
                message = "Status Code 201 - Created";
                break;
            case 202:
                message = "Status Code 202 - Accepted";
                break;
            case 204:
                message = "Status Code 204 - No Content";
                break;
            case 301:
                message = "Status Code 301 - Move data";
                break;
            case 401:
                message = "Status Code 401 - Unauthorized";
                break;
            case 403:
                message = "Status Code 404 - Not Found";
                break;
            case 404:
                message = "Status Code 404 - Forbidden";
                break;
            case 405:
                message = "Status Code 405 - Method Not Allowed";
                break;
            case 406:
                message = "Status Code 406 - Not Acceptable";
                break;
            case 412:
                message = "Status Code 412 - Precondition Failed";
                break;
            case 415:
                message = "Status Code 415 - Unsupported Media Type";
                break;
            case 500:
                message = "Status Code 500 - Internal Server Error";
                break;
            case 501:
                message = "Status Code 501 - Not Implemented";
                break;
            default:
                message = "Status Code " + code + " Server Issue";
                break;

        }

        return message;
	}
}
