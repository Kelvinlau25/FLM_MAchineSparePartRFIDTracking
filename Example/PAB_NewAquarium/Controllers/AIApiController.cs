using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using PAB_NewAquarium.Models;
using PAB_NewAquarium.Helpers;

namespace PAB_NewAquarium.Controllers
{
    public class AIApiController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> WordDetection(WordDetectionModel model)
        {
            try
            {
                WordDetectionHelper helper = new WordDetectionHelper();
                SimilarityResponseModel similarityResponseModel = new SimilarityResponseModel();
                similarityResponseModel = await helper.RetrieveWordResponseAsync(model);
                if (similarityResponseModel.score != -1)
                    return Json(similarityResponseModel);
                else
                    return Json("Error");
            }
            catch (Exception)
            {
                return Json("Error");
            }
        }
    }
}