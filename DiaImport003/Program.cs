using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using DiaImport003.model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using DiaImport003.model;
using System.Configuration;

namespace DiaImport003
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args);
            Console.ReadKey(true);

        }
        static async Task MainAsync(string[] args)
        {

            Console.WriteLine("start import");
            string dialogflowToken = ConfigurationManager.AppSettings["dialogflowToken"];
            string excelPath = ConfigurationManager.AppSettings["excelPath"];
            Application excelApp = new Application();
            if (excelApp == null)
            {
                Console.WriteLine("Excel is not installed!!");
                return;
            }
            Workbook excelBook = null;
            try
            {
                excelBook = excelApp.Workbooks.Open(@excelPath);
            }
            catch
            {
                Console.WriteLine("Excel is not installed!!");
            }

            _Worksheet excelSheet = excelBook.Sheets[1];
            Range excelRange = excelSheet.UsedRange;

            int rows = excelRange.Rows.Count;
            int cols = excelRange.Columns.Count;

            List<ExcelView> excelViewList = new List<ExcelView>();
            for (int i = 2; i <= rows; i++)
            {
                ExcelView excelView = new ExcelView();


                if (excelRange.Cells[i, 1] != null && excelRange.Cells[i, 1].Value2 != null)
                {
                    excelView.number = excelRange.Cells[i, 1].Value2.ToString();
                }
                else
                {
                    excelView.number = "-";
                }
                if (excelRange.Cells[i, 2] != null && excelRange.Cells[i, 2].Value2 != null)
                {
                    excelView.intent = excelRange.Cells[i, 2].Value2.ToString();
                }
                else
                {
                    excelView.intent = "-";
                }
                if (excelRange.Cells[i, 3] != null && excelRange.Cells[i, 3].Value2 != null)
                {
                    excelView.Trainingphrases = excelRange.Cells[i, 3].Value2.ToString();

                }
                else
                {

                    excelView.Trainingphrases = "-";
                }
                if (excelRange.Cells[i, 4] != null && excelRange.Cells[i, 4].Value2 != null)
                {
                    excelView.Responses = excelRange.Cells[i, 4].Value2.ToString();

                }
                else
                {
                    excelView.Responses = "-";

                }

                excelViewList.Add(excelView);
            }
            var query = from c in excelViewList
                        group c by c.number into g
                        select new { number = g.Key };
            Int32 n = query.Count();
            List<IntentView> intentViewList = new List<IntentView>();
            for (int i = 1; i <= n; i++)
            {
                IntentView intentView = new IntentView();
                List<ExcelView> excelViewListTemp = excelViewList.Where(w => w.number == Convert.ToString(i)).ToList();
                foreach (ExcelView excelView in excelViewListTemp)
                {
                    if (intentView.name.Equals("-"))
                    {
                        intentView.name = excelView.intent;
                        intentView.action = excelView.intent;
                    }
                    if (!excelView.Trainingphrases.Equals("-"))
                    {
                        intentView.input.Add(excelView.Trainingphrases);
                    }
                    if (!excelView.Responses.Equals("-"))
                    {
                        intentView.response.Add(excelView.Responses);
                    }
                }
                DialogFlowInsertModel dialogFlowModel = new DialogFlowInsertModel();
                dialogFlowModel.contexts = new List<string>();
                dialogFlowModel.events = new List<object>();
                dialogFlowModel.fallbackIntent = false;
                dialogFlowModel.name = intentView.name;
                dialogFlowModel.priority = 500000;
                dialogFlowModel.responses = new List<Respons>();
                dialogFlowModel.templates = new List<string>();
                dialogFlowModel.userSays = new List<UserSay>();
                dialogFlowModel.webhookForSlotFilling = false;
                dialogFlowModel.webhookUsed = false;

                Respons respons = new Respons();
                respons.action = intentView.name;
                respons.affectedContexts = new List<AffectedContext>();
                respons.defaultResponsePlatforms = new DefaultResponsePlatforms();
                respons.messages = new List<Message>();
                respons.parameters = new List<model.Parameter>();
                respons.resetContexts = false;

                Message message = new Message();
                message.type = 0;
                message.lang = "th";
                message.speech = new List<string>();


                foreach (String res in intentView.response)
                {
                    message.speech.Add(res);
                }

                respons.messages.Add(message);
                dialogFlowModel.responses.Add(respons);

                foreach (String input in intentView.input)
                {
                    UserSay userSay = new UserSay();
                    userSay.count = 0;
                    userSay.data = new List<Data>();
                    Data data = new Data();
                    data.text = input;
                    data.userDefined = false;
                    userSay.data.Add(data);
                    dialogFlowModel.userSays.Add(userSay);
                }
                String json = JsonConvert.SerializeObject(dialogFlowModel);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + dialogflowToken);
                    try
                    {

                        HttpResponseMessage responseGetInent = await client.GetAsync($"https://api.dialogflow.com/v1/intents?v=20150910");
                        if (responseGetInent.IsSuccessStatusCode)
                        {
                            HttpContent content = responseGetInent.Content;
                            string result = await content.ReadAsStringAsync();
                            List<InentModel> intentList = (List<InentModel>)JsonConvert.DeserializeObject(result, typeof(IList<InentModel>));

                            InentModel inentModel = intentList.Where(T => T.name.Equals(dialogFlowModel.name)).FirstOrDefault();
                            if (inentModel == null)
                            {
                                HttpResponseMessage res = await client.PostAsync($"https://api.dialogflow.com/v1/intents?v=20150910", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                                if (res.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("intent " + intentView.name + " insert succeeded");
                                }
                                else
                                {
                                    Console.WriteLine("intent " + intentView.name + " insert failed");
                                }

                            }
                            else
                            {
                                HttpResponseMessage res = await client.PutAsync($"https://api.dialogflow.com/v1/intents/" + inentModel.id+ "?v=20150910", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                                if (res.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("intent " + intentView.name + " update succeeded");
                                }
                                else
                                {
                                    Console.WriteLine("intent " + intentView.name + " update failed");
                                }
                            }
                            //==============================



                        }
                        else
                        {
                            Console.WriteLine("intent " + intentView.name + " failed");
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("intent " + intentView.name + " failed");
                    }
                }
            }
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            Console.WriteLine("finished");
            Console.ReadLine();

            Console.ReadKey(true);
        }
    }
}
