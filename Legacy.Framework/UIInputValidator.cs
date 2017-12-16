using System;
using UnityEngine;

[RequireComponent(typeof(UIInput))]
[AddComponentMenu("NGUI/Interaction/Input Validator")]
public class UIInputValidator : MonoBehaviour
{
	public Validation logic;

	private void Start()
	{
		GetComponent<UIInput>().validator = new UIInput.Validator(Validate);
	}

	private Char Validate(String text, Char ch)
	{
		if (logic == Validation.None || !enabled)
		{
			return ch;
		}
		if (logic == Validation.Integer)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && text.Length == 0)
			{
				return ch;
			}
		}
		else if (logic == Validation.Float)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && text.Length == 0)
			{
				return ch;
			}
			if (ch == '.' && !text.Contains("."))
			{
				return ch;
			}
		}
		else if (logic == Validation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (logic == Validation.Username)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return (Char)(ch - 'A' + 'a');
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (logic == Validation.Name)
		{
			Char c = (text.Length <= 0) ? ' ' : text[text.Length - 1];
			if (ch >= 'a' && ch <= 'z')
			{
				if (c == ' ')
				{
					return (Char)(ch - 'a' + 'A');
				}
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				if (c != ' ' && c != '\'')
				{
					return (Char)(ch - 'A' + 'a');
				}
				return ch;
			}
			else if (ch == '\'')
			{
				if (c != ' ' && c != '\'' && !text.Contains("'"))
				{
					return ch;
				}
			}
			else if (ch == ' ' && c != ' ' && c != '\'')
			{
				return ch;
			}
		}
		return '\0';
	}

	public enum Validation
	{
		None,
		Integer,
		Float,
		Alphanumeric,
		Username,
		Name
	}
}
