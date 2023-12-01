using System.Text.RegularExpressions;
using Lighter.NET.Common;
using Microsoft.Extensions.Localization;

namespace Lighter.NET.Helpers
{
    public class ValidationHelper
    {
        /// <summary>
        /// Validator的語系轉換器
        /// </summary>
        public static IStringLocalizer ValidatorLocalizer { get; set; } = LangHelper.GetLocalizer<DefaultResource>();
 
        /// <summary>
        /// 檢核：不可超過當下時間
        /// </summary>
        /// <param name="inputTime">時間值</param>
        /// <param name="errMsg">回傳錯誤訊息</param>
        /// <param name="toleranceSeconds">容許誤差秒數</param>
        /// <param name="checkRequired">是否檢核必填</param>
        /// <returns>是否檢核通過</returns>
        public static bool NotOverNow(DateTime? inputTime, out string errMsg, int toleranceSeconds = 0, bool checkRequired = false)
        {
            errMsg = "";

            if (checkRequired && (inputTime.HasValue == false || inputTime.Value.Equals(DateTime.MinValue)))
            {
                errMsg = ValidatorLocalizer["未填寫"];
                return false;
            }

            DateTime timeLimit = DateTime.Now.AddSeconds(toleranceSeconds);
            if (inputTime.HasValue && inputTime.Value > timeLimit)
            {
                errMsg = ValidatorLocalizer["不可大於現在時間"];
            }

            return errMsg == "";
        }

        /// <summary>
        /// 檢核電話號碼
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="errMsg"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="example"></param>
        /// <param name="checkRequired"></param>
        /// <returns></returns>
        public static bool CheckPhoneNumber(string phoneNumber,out string errMsg, int minLength=8,int maxLength = 25, string example = "0955-012345", bool checkRequired = false)
        {
            errMsg = "";
            if (checkRequired && string.IsNullOrEmpty(phoneNumber))
            {
                errMsg = ValidatorLocalizer["未填寫"];
                return false;
            }

            if (string.IsNullOrEmpty(phoneNumber)) phoneNumber = "";

            //忽略字元
            char[] ignoreChars = new char[] { ' ', '-', '(', ')', '#' };

            //檢核長度
            string phoneFiltered = phoneNumber.FilterOut(ignoreChars);
            if (phoneFiltered.Length < minLength) 
            { 
                errMsg = ValidatorLocalizer["長度至少{0}個數字", minLength]; 
                return false;
            }
            if (phoneFiltered.Length > maxLength) 
            { 
                errMsg = ValidatorLocalizer["長度上限{0}個數字", maxLength]; 
                return false;
            }
            //檢核格式
            string pattern = @"^\+?\(?[0-9]+\)?[-\s]?[0-9]+[-\s]?[0-9]+#?[0-9]+$";
            if(!Regex.IsMatch(phoneNumber, pattern))
            {
                errMsg = ValidatorLocalizer["格式錯誤!參考範例：{0}", example];
                return false;
            }
            //檢核無效值
            //(1)全部重複字元
            if (phoneNumber.IsAllRepeatCharacter(ignoreChars))
            {
                errMsg = ValidatorLocalizer["格式錯誤!參考範例：{0}", example];
                return false;
            }
            //(2)同範例字串
            if(phoneNumber == example)
            {
                errMsg = ValidatorLocalizer["號碼有誤，請重新輸入"];
                return false;
            }

            return errMsg == "";
            
        }

        /// <summary>
        /// 檢核電話號碼
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="errMsg"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="example"></param>
        /// <param name="checkRequired"></param>
        /// <returns></returns>
        public static bool CheckPhone(string phoneNumber, out string errMsg, int minLength = 8, int maxLength = 25, string example = "04 1234-5678", bool checkRequired = false)
        {
            errMsg = "";

            if (checkRequired && string.IsNullOrEmpty(phoneNumber))
            {
                errMsg = ValidatorLocalizer["未填寫"];
                return false;
            }

            if(string.IsNullOrEmpty(phoneNumber)) phoneNumber= "";

            //忽略字元
            char[] ignoreChars = new char[] { ' ', '-', '(', ')','#' };

            //檢核長度
            string phoneFiltered = phoneNumber.FilterOut(ignoreChars);
            if (phoneFiltered.Length < minLength) 
            { 
                errMsg = ValidatorLocalizer["長度至少{0}個數字", minLength];
                return false;
            }
            if (phoneFiltered.Length > maxLength) 
            { 
                errMsg = ValidatorLocalizer["長度上限{0}個數字", maxLength]; 
                return false;
            }
            //檢核格式
            string pattern = @"^\+?\(?[0-9]+\)?[-\s]?[0-9]+[-\s]?[0-9]+#?[0-9]+$";
            if (!Regex.IsMatch(phoneNumber, pattern))
            {
                errMsg = ValidatorLocalizer["格式錯誤!參考範例：{0}", example];
                return false;
            }
            //檢核無效值
            //(1)全部重複字元
            if (phoneNumber.IsAllRepeatCharacter(ignoreChars))
            {
                errMsg = ValidatorLocalizer["格式錯誤!參考範例：{0}", example];
                return false;
            }
            //(2)同範例字串
            if (phoneNumber == example)
            {
                errMsg = ValidatorLocalizer["號碼有誤，請重新輸入"];
                return false;
            }

            return errMsg == "";

        }

        /// <summary>
        /// 檢核行動電話
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="errMsg"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="example"></param>
        /// <param name="checkRequired"></param>
        /// <returns></returns>
        public static bool CheckMobile(string mobileNumber, out string errMsg, int minLength = 10, int maxLength = 25, string example = "0955-012345", bool checkRequired = false)
        {
            errMsg = "";
            if (checkRequired && string.IsNullOrEmpty(mobileNumber))
            {
                errMsg = ValidatorLocalizer["未填寫"];
                return false;
            }

            if (string.IsNullOrEmpty(mobileNumber)) mobileNumber = "";

            //忽略字元
            char[] ignoreChars = new char[] { ' ', '-', '(', ')', '#' };

            //檢核長度
            string phoneFiltered = mobileNumber.FilterOut(ignoreChars);
            if (phoneFiltered.Length < minLength) 
            { 
                errMsg = ValidatorLocalizer["長度至少{0}個數字", minLength]; 
                return false;
            }
            if (phoneFiltered.Length > maxLength) 
            { 
                errMsg = ValidatorLocalizer["長度上限{0}個數字", maxLength]; 
                return false;
            }
            //檢核格式
            string pattern = @"^\+?\(?[0-9]+\)?[-\s]?[0-9]+[-\s]?[0-9]+$";
            if (!Regex.IsMatch(mobileNumber, pattern))
            {
                errMsg = ValidatorLocalizer["格式錯誤!參考範例：{0}", example];
                return false;
            }
            //檢核無效值
            //(1)全部重複字元
            if (mobileNumber.IsAllRepeatCharacter(ignoreChars))
            {
                errMsg = ValidatorLocalizer["格式錯誤!參考範例：{0}", example];
                return false;
            }
            //(2)同範例字串
            if (mobileNumber == example)
            {
                errMsg = ValidatorLocalizer["號碼有誤，請重新輸入"];
                return false;
            }

            return errMsg == "";

        }

        /// <summary>
        /// 檢核Email格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool CheckEmail(string email, bool isRequired = true)
        {
            string trimed = (email == null) ? "" : email.Trim();
			if (!isRequired && trimed == "") return true;
			var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
			var regex = new Regex(pattern);
			return regex.IsMatch(trimed);
		}

		/// <summary>
		/// 檢核證件號碼
		/// </summary>
		/// <param name="cid">證件號碼</param>
		/// <param name="cidType">證件號碼(身份證or統一編號or二者一起)</param>
		/// <param name="errMsg">回傳錯誤訊息</param>
		/// <param name="isRequired">是否必填欄位</param>
		/// <returns></returns>
		public static bool CheckCID(string cid, CIDType cidType, out string errMsg, bool isRequired = true) {
            errMsg = "";
            switch (cidType)
            {
                case CIDType.PID:
                    return CheckPID(cid, out errMsg, isRequired);
                case CIDType.UBN:
                    return CheckUBN(cid, out errMsg, isRequired);
                case CIDType.Both_PID_UBN:
                    return CheckBoth_PID_UBN(cid, out errMsg, isRequired);
                default:
                    return true;
                    
            }

        }

        /// <summary>
        /// 檢核身份證、居留證(舊式)
        /// </summary>
        /// <param name="pid">證號</param>
        /// <param name="errMsg">回傳錯誤訊息</param>
        /// <param name="isRequired">是否必填欄位</param>
        /// <returns></returns>
        public static bool CheckPID(string pid , out string errMsg, bool isRequired=true)
        {
            /*
             *此函式參考前人EduFund專案的程式
             */
			errMsg = "";
			int[] idnum = { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
			string idno = pid == null ? string.Empty : pid.Trim().ToUpper(); //將字母全轉為大寫
			int sum;
			if (!isRequired && idno == "") return true;

			if ( isRequired && idno =="")
			{
				errMsg = "未填寫";
			}
			else if (idno.Length < 10)
			{
				errMsg = "字數不足10碼";
			}
			else if (idno.Length > 10)
            {
				errMsg = "字數超過10碼";
			}
			else
			{
                //檢核格式
                bool formatOK = true;
                string pattern = @"^[A-Za-z]{1,1}[0-9]+$";
                if (!Regex.IsMatch(idno, pattern))
                {
                    formatOK= false;
                    errMsg = ValidatorLocalizer["請填寫正確格式：1碼英文字母+9碼數字"];
                }

                if(formatOK) 
                { 
                    #region 居留證判斷-舊式
                    if (idno[0] >= 'A' && idno[0] <= 'Z' && idno[1] >= 'A' && idno[1] <= 'D')
				    {/*
                        sum = (idnum[idno[0] - 'A'] / 10 * 1) + (idnum[idno[0] - 'A'] % 10 * 9) + (idnum[idno[1] - 'A'] % 10 * 8);
                        for (int i = 1; i < 8; i++)
                        {
                            sum += ((idno[i + 1] - '0') * (8 - i) % 10);
                        }
                        if ((idno[9] - '0') != 10 - (sum % 10))
                        {
                            message = "身份證字號有誤";
                        }
                     */
					    string Uppid = idno.ToUpper();

					    var nbs = new int[11];
					    nbs[0] = idnum[Uppid[0] - 65] / 10;
					    nbs[1] = idnum[Uppid[0] - 65] % 10;
					    nbs[2] = idnum[Uppid[1] - 65] % 10;
					    sum = nbs[0];

					    for (var i = 1; i <= 9; i++)
					    {
						    if (i > 1)
						    {
							    nbs[i + 1] = Uppid[i] - 48;
						    }

						    sum += nbs[i] * (10 - i);
					    }
					    if ((sum + nbs[10]) % 10 != 0)
					    {
						    errMsg = "居留證字號有誤";
					    }
				    }
				    #endregion
				    //#region 居留證判斷-新式 -20210416新增
				    //else if (idno[0] >= 'A' && idno[0] <= 'Z')
				    //{
				    //    if ((idno[1] - '0') == 8 || (idno[1] - '0') == 9)
				    //    {
				    //        sum = (idnum[idno[0] - 'A'] / 10 * 1) + (idnum[idno[0] - 'A'] % 10 * 9);
				    //        for (int i = 1; i < 9; i++)
				    //        {
				    //            sum += (idno[i] - '0') * (9 - i);
				    //        }
				    //        sum += (idno[9] - '0');

				    //        if (sum % 10 != 0)
				    //        {
				    //            message = "居留證字號有誤";
				    //        }
				    //    }
				    //}
				    //#endregion
				    #region 身份證判斷
				    else if (idno[0] >= 'A' && idno[0] <= 'Z')
				    {
					    //檢查第一個數值是否為1.2(判斷性別)
					    if ((idno[1] - '0') == 1 || (idno[1] - '0') == 2)
					    {
						    sum = (idnum[idno[0] - 'A'] / 10 * 1) + (idnum[idno[0] - 'A'] % 10 * 9);
						    for (int i = 1; i < 9; i++)
						    {
							    sum += (idno[i] - '0') * (9 - i);
						    }
						    sum += (idno[9] - '0');

						    if (sum % 10 != 0)
						    {
							    errMsg = "身份證字號有誤";
						    }

					    }
					    else if ((idno[1] - '0') >= 3 && (idno[1] - '0') <= 7)
					    {
						    errMsg = "身份證字號有誤";
					    }
					    //檢查第一個數值是否為8.9(判斷性別)-2021.04.20新增
					    #region 居留證判斷-新式
					    if ((idno[1] - '0') == 8 || (idno[1] - '0') == 9)
					    {
						    sum = (idnum[idno[0] - 'A'] / 10 * 1) + (idnum[idno[0] - 'A'] % 10 * 9);
						    for (int i = 1; i < 9; i++)
						    {
							    sum += (idno[i] - '0') * (9 - i);
						    }
						    sum += (idno[9] - '0');

						    if (sum % 10 != 0)
						    {
							    errMsg = "居留證字號有誤";
						    }
					    }
					    #endregion
				    }
				    else
				    {
					    errMsg = "身份證字號有誤";
				    }
				    #endregion                
                }

			}
			return errMsg == "";
        }

		/// <summary>
		/// 檢核公司統一編號
		/// </summary>
		/// <param name="ubn">統一編號</param>
		/// <param name="errMsg">回傳錯誤訊息</param>
		/// <param name="isRequired">是否必填欄位</param>
		/// <returns></returns>
		public static bool CheckUBN(string ubn,out string errMsg, bool isRequired = true)
        {
			/*
             * 依照財政部110-12-22公佈之新版規則(112年4月以後全面生效)
             */
			errMsg = "";
            string ubnStr = (ubn == null) ? "" : ubn.Trim();
            if (!isRequired && ubnStr == "") return true;

			if (isRequired && ubnStr == "")
			{
				errMsg = "未填寫";
            }
            else if(ubnStr.Length < 8)
            {
                errMsg = "字數不足8碼";
            }
            else if(ubnStr.Length > 8)
            {
				errMsg = "字數超過8碼";
			}
			else if (int.TryParse(ubnStr ,out int _) == false)
			{
				errMsg = "統一編號必須是數字";
            }
            else
            {
                int[] ubnArr = ubnStr.ToCharArray().Select(x=>int.Parse(x.ToString())).ToArray(); //拆成個別數字
                int[] chkArr = new int[] {1,2,1,2,1,2,4,1 }; //對照的個別數字檢核子
                int sum = 0;
                int p = 0;
                for(int i=0;i<8;i++)
                {
                    p = ubnArr[i] * chkArr[i];
                    if (p < 10)
                    {
                        sum += p;
                    }
                    else
                    {
                        sum += (p/10 + p%10);
                    }
                }
                if (sum % 5 == 0)
                {
                    return true;
                }
                else if (ubnArr[6]==7 && (sum + 1) % 5 == 0)
                {
                    return true;
                }
                else
                {
                    errMsg = "統一編號不正確";
                }
            }

			return errMsg == "";
		}

		/// <summary>
		/// 檢核身份證、居留證(舊式)或統一編號
		/// </summary>
		/// <param name="pid_or_ubn"></param>
		/// <param name="errMsg"></param>
		/// <param name="isRequired">是否必填欄位</param>
		/// <returns></returns>
		public static bool CheckBoth_PID_UBN(string pid_or_ubn, out string errMsg, bool isRequired = true)
        {
            errMsg = "";
            string trimed = (pid_or_ubn == null) ? "" : pid_or_ubn.Trim();

            //身份證
			if (CheckPID(trimed, out string _, isRequired))
            {
                return true;
            }
            else
            {
                //統一編號
                if(CheckUBN(trimed, out string _, isRequired))
                {
                    return true;
                }
                else
                {
                    if(isRequired && trimed == "")
                    {
                        errMsg = "未填寫";
                    }
                    else if (trimed.Length < 8)
                    {
                        errMsg = "字數不足、格式錯誤或號碼不正確";
                    }
                    else 
                    {
                        errMsg = "字數、格式錯誤或號碼不正確";
                    }
                }
            }

            return errMsg == "";
        }
    }
}
