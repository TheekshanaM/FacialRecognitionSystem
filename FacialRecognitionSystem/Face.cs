using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Web.UI;
using System.Linq;
using System.Threading;


namespace FaceAPIFunctions
{
    public class Face0
    {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("eda91d84d8d74e99b5afa36073cea990", "https://southeastasia.api.cognitive.microsoft.com/face/v1.0");
        String groupId = "001";
       

        public Face0()
        {

        }

        
     

        public async Task<String> register(String path,String UserId)
        {
            var faces= await detectFace(path,UserId);
            var faceIds = faces.Select(face => face.FaceId).ToArray();
            if (faces.Length == 0)
            {
                return ("No faces Detected");
            }
            if (faces.Length > 1)
            {
                return ("More than one face detected");
            }
            else
            {
                var result = await addperson(path, UserId);
                return result;
            }
            

        }
        private async Task<Face[]> detectFace(String path, String personId)
        {

            using (Stream s = File.OpenRead(path))
            {

                var faces = await faceServiceClient.DetectAsync(s);
                return faces;
            }                  

        }
        public async Task<String> detectFaceTrue(String path)
        {

            using (Stream s = File.OpenRead(path))
            {

                var faces = await faceServiceClient.DetectAsync(s);
                if (faces.Length == 0)
                {
                    return "no face detected";
                }
                if (faces.Length > 1)
                {
                    return "more than one face detected";
                }
                else
                {
                    return "face detected successfully";
                }
            }

        }
        private async Task<String> addperson(String path, String personId)
        {
            try
            {
                CreatePersonResult persons = await faceServiceClient.CreatePersonAsync(groupId, personId);

                var personIds = persons.PersonId;
                using (Stream s = File.OpenRead(path))
                {
                    var persistedperson = await faceServiceClient.AddPersonFaceAsync(groupId, personIds, s);
                    await faceServiceClient.TrainPersonGroupAsync(groupId);
                    return "Success";
                }
            }
            catch ( Exception e)
            {
                return e.Message;
                
            }

        }
       
        public async Task<String> search(String path)
        {
            StringBuilder sb = new StringBuilder();
            using (Stream s = File.OpenRead(path))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                if (faces.Length == 0) { return "no face detected"; }
                else
                {
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await faceServiceClient.IdentifyAsync(groupId, faceIds);
                    foreach (var identifyResult in results)
                    {
                        
                        if (identifyResult.Candidates.Length == 0)
                        {
                            sb.Append(" No one identified ");
                        }
                        else
                        {
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(groupId, candidateId);
                            sb.Append("Identified as  " + person.Name + "          ");
                        }
                    }
                    return sb.ToString(); 
                }
            }
        }

    }
            
 
}