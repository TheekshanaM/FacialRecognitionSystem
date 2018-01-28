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

        
     

        public async Task<String> register(byte[] imgData, int Id)
        {
            
            using (var path = new MemoryStream(imgData))
            {
                string UserId = Id.ToString();
                var faces = await detectFace(path, UserId);
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
                    using (var path2 = new MemoryStream(imgData))
                    {
                        var result = await addperson(path2, UserId);
                        return result;
                    }
                }
            }

        }
        private async Task<Face[]> detectFace(Stream path, String personId)
        {

            using (path)
            {

                var faces = await faceServiceClient.DetectAsync(path);
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
        private async Task<String> addperson(Stream path, String personId)
        {
            try
            {
                CreatePersonResult persons = await faceServiceClient.CreatePersonAsync(groupId, personId);

                var personIds = persons.PersonId;
                using(path)
                {
                    var persistedperson = await faceServiceClient.AddPersonFaceAsync(groupId, personIds, path);
                    await faceServiceClient.TrainPersonGroupAsync(groupId);
                    return "Success";
                }
            }
            catch ( Exception e)
            {
                return e.Message;
                
            }

        }
       
        public async Task<int[]> search(Stream path)
        {
            StringBuilder sb = new StringBuilder();
            using (path)
            {
                int[] id = new int[5];
                int i = 0;
                var faces = await faceServiceClient.DetectAsync(path);
                if (faces.Length == 0) {
                    id[i]=-1;
                }
                else
                {
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await faceServiceClient.IdentifyAsync(groupId, faceIds);
                    foreach (var identifyResult in results)
                    {
                        
                        if (identifyResult.Candidates.Length == 0)
                        {
                            id[i] = 0;
                            if (i == 4) { break; }
                            i++;
                        }
                        else
                        {
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(groupId, candidateId);
                            try
                            {
                                id[i] = Convert.ToInt32(person.Name);
                                if (i == 4) { break; }
                                i++;
                            }
                            catch
                            {
                                if (i == 4) { break; }
                                i++;
                            }
                            
                        }
                    }
                     
                }
                return id;
            }
        }


        public async Task<int> searchFirst(Stream path)
        {
            
            using (path)
            {
                var faces = await faceServiceClient.DetectAsync(path);
                if (faces.Length == 0) { return 0; }//no one detected
                if (faces.Length > 1) { return 1; }//more than one person detected
                else
                {
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await faceServiceClient.IdentifyAsync(groupId, faceIds);
                    foreach (var identifyResult in results)
                    {

                        if (identifyResult.Candidates.Length == 0)
                        {
                            return 3;// success
                        }
                        else
                        {
                            return 2;//there is an existing person
                        }
                    }return 0;
                    
                }
            }
        }

    }
            
 
}