using System.Drawing;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;

// Import namespaces


namespace ComputerVision.DetectFaces
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServiceKey"];

                // Get image
                string imageFile = "images/people.jpg";
                if (args.Length > 0)
                {
                    imageFile = args[0];
                }

                // Authenticate Azure AI Vision client
                ImageAnalysisClient cvClient = new(
                    new Uri(aiSvcEndpoint),
                    new AzureKeyCredential(aiSvcKey));


                // Analyze image
                AnalyzeImage(imageFile, cvClient);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void AnalyzeImage(string imageFile, ImageAnalysisClient client)
        {
            Console.WriteLine($"\nAnalyzing {imageFile} \n");

            // Use a file stream to pass the image data to the analyze call
            using FileStream stream = new FileStream(imageFile,
                                                     FileMode.Open);

            // Get result with specified features to be retrieved (PEOPLE)
            ImageAnalysisResult result = client.Analyze(
            BinaryData.FromStream(stream),
            VisualFeatures.People);


            // Close the stream
            stream.Close();

            // Get people in the image
            if (result.People.Values.Count > 0)
            {
                Console.WriteLine($" People:");

                // Prepare image for drawing
                System.Drawing.Image image = System.Drawing.Image.FromFile(imageFile);
                Graphics graphics = Graphics.FromImage(image);
                Pen pen = new Pen(Color.Cyan, 3);

                // Draw bounding box around detected people
                foreach (DetectedPerson person in result.People.Values)
                {
                    if (person.Confidence > 0.5)
                    {
                        // Draw object bounding box
                        var r = person.BoundingBox;
                        Rectangle rect = new Rectangle(r.X, r.Y, r.Width, r.Height);
                        graphics.DrawRectangle(pen, rect);
                    }

                    // Return the confidence of the person detected
                    Console.WriteLine($"Bounding box {person.BoundingBox}, Confidence: {person.Confidence:F2}");
                }


                // Save annotated image
                String output_file = "people.jpg";
                image.Save(output_file);
                Console.WriteLine("  Results saved in " + output_file + "\n");
            }

        }


    }
}
