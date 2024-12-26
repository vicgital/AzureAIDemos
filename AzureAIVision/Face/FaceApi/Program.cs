using System.Drawing;
// Import namespaces
using Azure;
using Azure.AI.Vision.Face;
using Microsoft.Extensions.Configuration;


namespace FaceApi
{
    class Program
    {

        private static FaceClient faceClient;
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["AIServicesEndpoint"];
                string cogSvcKey = configuration["AIServiceKey"];

                // Authenticate Face client
                faceClient = new FaceClient(new Uri(cogSvcEndpoint), new AzureKeyCredential(cogSvcKey));


                // Menu for face functions
                Console.WriteLine("1: Detect faces\nAny other key to quit");
                Console.WriteLine("Enter a number:");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        await DetectFaces("images/person1.jpg");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task DetectFaces(string imageFile)
        {
            Console.WriteLine($"Detecting faces in {imageFile}");

            // Specify facial features to be retrieved
            FaceAttributeType[] features = new FaceAttributeType[]
             {
                 FaceAttributeType.Detection03.HeadPose,
                 FaceAttributeType.Detection03.Blur,
                 FaceAttributeType.Detection03.Mask
             };


            // Get faces
            using var imageData = File.OpenRead(imageFile);


            var response = await faceClient.DetectAsync(
                BinaryData.FromStream(imageData),
                FaceDetectionModel.Detection03,
                FaceRecognitionModel.Recognition04,
                returnFaceId: false,
                returnFaceAttributes: features);

            // Get detected faces
            IReadOnlyList<FaceDetectionResult> detected_faces = response.Value;

            if (detected_faces.Count() > 0)
            {
                Console.WriteLine($"{detected_faces.Count()} faces detected.");

                // Prepare image for drawing
                Image image = Image.FromFile(imageFile);
                Graphics graphics = Graphics.FromImage(image);
                Pen pen = new Pen(Color.LightGreen, 3);
                Font font = new Font("Arial", 4);
                SolidBrush brush = new SolidBrush(Color.White);
                int faceCount = 0;

                // Draw and annotate each face
                foreach (var face in detected_faces)
                {
                    faceCount++;
                    Console.WriteLine($"\nFace number {faceCount}");

                    // Get face properties
                    Console.WriteLine($" - Head Pose (Yaw): {face.FaceAttributes.HeadPose.Yaw}");
                    Console.WriteLine($" - Head Pose (Pitch): {face.FaceAttributes.HeadPose.Pitch}");
                    Console.WriteLine($" - Head Pose (Roll): {face.FaceAttributes.HeadPose.Roll}");
                    Console.WriteLine($" - Blur: {face.FaceAttributes.Blur.BlurLevel}");
                    Console.WriteLine($" - Mask: {face.FaceAttributes.Mask.Type}");

                    // Draw and annotate face
                    var r = face.FaceRectangle;
                    Rectangle rect = new Rectangle(r.Left, r.Top, r.Width, r.Height);
                    graphics.DrawRectangle(pen, rect);
                    string annotation = $"Face number {faceCount}";
                    graphics.DrawString(annotation, font, brush, r.Left, r.Top);
                }

                // Save annotated image
                String output_file = "detected_faces.jpg";
                image.Save(output_file);
                Console.WriteLine(" Results saved in " + output_file);
            }



        }
    }
}
