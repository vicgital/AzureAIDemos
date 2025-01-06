﻿using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
// Import namespaces
using Microsoft.CognitiveServices.Speech.Audio;
using System.Media;


namespace AzureAISpeechDemo
{
    class Program
    {
        private static SpeechConfig speechConfig;
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcKey = configuration["SpeechKey"];
                string aiSvcRegion = configuration["SpeechRegion"];

                // Configure speech service
                speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);
                Console.WriteLine("Ready to use speech service in " + speechConfig.Region);

                // Configure voice
                speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";


                // Get spoken input
                string command = "";
                command = await TranscribeCommand();
                if (command.ToLower() == "what time is it?")
                {
                    await TellTime();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<string> TranscribeCommand()
        {
            string command = "";

            // Configure speech recognition
            using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
            Console.WriteLine("Speak now...");

            // Using Audio input from File
            //string audioFile = "time.wav";
            //SoundPlayer wavPlayer = new SoundPlayer(audioFile);
            //wavPlayer.Play();
            //using AudioConfig audioConfig = AudioConfig.FromWavFileInput(audioFile);
            //using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);



            // Process speech input
            SpeechRecognitionResult speech = await speechRecognizer.RecognizeOnceAsync();
            if (speech.Reason == ResultReason.RecognizedSpeech)
            {
                command = speech.Text;
                Console.WriteLine(command);
            }
            else
            {
                Console.WriteLine(speech.Reason);
                if (speech.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(speech);
                    Console.WriteLine(cancellation.Reason);
                    Console.WriteLine(cancellation.ErrorDetails);
                }
            }


            // Return the command
            return command;
        }

        static async Task TellTime()
        {
            var now = DateTime.Now;
            string responseText = "The time is " + now.Hour.ToString() + ":" + now.Minute.ToString("D2");

            // Configure speech synthesis
            speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

            // Synthesize spoken output
            //------- REGULAR ------
            //SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(responseText);
            //if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            //{
            //    Console.WriteLine(speak.Reason);
            //}


            //------- WITH SSML ------
            string responseSsml = $@"
             <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                 <voice name='en-GB-LibbyNeural'>
                     {responseText}
                     <break strength='weak'/>
                     Time to end this lab!
                 </voice>
             </speak>";
            SpeechSynthesisResult speak = await speechSynthesizer.SpeakSsmlAsync(responseSsml);
            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(speak.Reason);
            }

            // Print the response
            Console.WriteLine(responseText);
        }

    }
}
