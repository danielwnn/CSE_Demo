using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceApiSamples
{
    public class FaceIdentify
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Started...");

                test();

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }            
        }

        static async void test()
        {
            var faceServiceClient = new FaceServiceClient(
                    "<your face api key>", 
                    "https://westus.api.cognitive.microsoft.com/face/v1.0"
                );

            
            // Create an empty PersonGroup
            string personGroupId = "Myfriends";

            /*
            await faceServiceClient.CreatePersonGroupAsync(personGroupId, "My Friends");

            // Define Anna
            CreatePersonResult person1 = await faceServiceClient.CreatePersonAsync(
                // Id of the PersonGroup that the person belonged to
                personGroupId,
                // Name of the person
                "Cristiano Ronaldo"
            );

            // Directory contains image files of Ronaldo
            string friend1ImageDir = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\images\Ronaldo\");

            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, person1.PersonId, s);
                }
            }

            // training
            await faceServiceClient.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }   
            */


            // identify a person
            string testImageFile = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\images\test_pics\cristiano-ronaldo.jpg");

            using (Stream s = File.OpenRead(testImageFile))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();

                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                foreach (var identifyResult in results)
                {
                    Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        Console.WriteLine("No one identified");
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                        Console.WriteLine("Identified as {0}", person.Name);
                    }
                }
            }
            // end
        }
    }
}
