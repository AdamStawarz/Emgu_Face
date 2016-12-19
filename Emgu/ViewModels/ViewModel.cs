using Emgu.Models;
using System.Windows.Input;
using Emgu.Helpers;
using Emgu.CV;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System;
using Emgu.CV.Structure;
using System.Windows.Threading;
using System.Drawing;
using System.Collections.Generic;
using Project_FaceRecognition;
using System.IO;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using System.Threading.Tasks;

namespace Emgu.ViewModels
{
   

    public class ViewModel : ViewModelBase
    {
       //
        Capture capture = new Capture(); //create a camera captue
        Image<Gray, Byte> Sub_grayFrame = null;
        EigenFaceRecognizer _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
        DataStoreAccess _dataStoreAccess = new DataStoreAccess();

        #region konstruktory
        public ViewModel()
        {            
            StartCameraButton = new RelayCommand(StartCamCommandExecute);         //Tworzę nowy Button//
            SaveCurrentImageButton = new RelayCommand(SaveCurrentImageCommandExecute);
            TrainRecognizerButton = new RelayCommand(TrainRecognizerCommandExecute);

            //TrainRecognizer();            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Mat ImageFrame = capture.QueryFrame();  // Now is Mat for queryframe
            Image<Gray, Byte> grayFrame = ImageFrame.ToImage<Gray, Byte>(); // ready to use image processing
            
            try
            {

                var _cascadeClassifier = new CascadeClassifier(@"C:\Emgu\Emgu\haarcascade_frontalface_default.xml");
                
                if (grayFrame != null)
                {
                    var faces = _cascadeClassifier.DetectMultiScale(grayFrame, 1.1, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200)); //the actual face detection happens here
                    if (faces !=null)
                    foreach (var face in faces)
                    {
                        grayFrame.Draw(face, new Gray(), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                        Sub_grayFrame = grayFrame.GetSubRect(face);

                            

                            myCamera = ToBitmapSource(ImageFrame);
                            myCamera1 = ToBitmapSource(grayFrame);

                            //label = RecognizeUser(Sub_grayFrame);
                        }
                }

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }                 
               
           
        }

        public void TrainRecognizer()
        {
            
                var allFaces = _dataStoreAccess.CallFaces("ALL_USERS");
                if (allFaces != null)
                {
                    var faceImages = new Image<Gray, byte>[allFaces.Count];
                    var faceLabels = new int[allFaces.Count];
                    for (int i = 0; i < allFaces.Count; i++)
                    {
                        Stream stream = new MemoryStream();
                        stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                        var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                        faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                        faceLabels[i] = allFaces[i].UserId;
                    }


                    _faceRecognizer.Train(faceImages, faceLabels);
                    _faceRecognizer.Save(@"C:\Users\Dom\Documents\Visual Studio 2015\Projects\Emgu\Emgu\Faces\recognizerFilePath\file.yaml");
                } 
        }
                
        public string RecognizeUser(Image<Gray, byte> userImage)
        {
           
                        _faceRecognizer.Load(@"C:\Users\Dom\Documents\Visual Studio 2015\Projects\Emgu\Emgu\Faces\recognizerFilePath\file.yaml");
                        var result = _faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
                        return result.Label.ToString();
                   
        }



        #endregion

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        

        #region Przyciski

        // Do nowego przycisku należy stworzyc properties i "ciało" przycisku gdzie wykonywana jest akcja

        public ICommand StartCameraButton { get; set; }
        private void StartCamCommandExecute(object obj)
        {
            var timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();

        }

        public ICommand SaveCurrentImageButton { get; set;}
        private void SaveCurrentImageCommandExecute(object obj)
        {            
            var faceToSave = Sub_grayFrame;
            Byte[] file;                        
            IDataStoreAccess dataStore = new DataStoreAccess();

            var username = name.ToLower().Trim();

            for (int i = 0; i<10; i++)
            {
                    var filePath = string.Format(@"C:\Users\Dom\Documents\Visual Studio 2015\Projects\Emgu\Emgu\Faces\{0}.bmp", username);
                    faceToSave.ToBitmap().Save(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            file = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    var result = dataStore.SaveFace(username, file);
            }
                    
                    MessageBox.Show("Save Result");               
        }

        public ICommand TrainRecognizerButton { get; set; }
        async private void TrainRecognizerCommandExecute(object obj)
        {
            TrainRecognizer();
        }

        #endregion



        private BitmapSource _myCamera;
        public BitmapSource myCamera
        {
            get
            {
                return _myCamera;
            }
            set
            {
                _myCamera = value;
                OnPropertyChanged("myCamera");
            }
        }

        private BitmapSource _myCamera1;
        public BitmapSource myCamera1
        {
            get
            {
                return _myCamera1;
            }
            set
            {
                _myCamera1 = value;
                OnPropertyChanged("myCamera1");
            }
        }

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }
        private string _label;
        public string label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                OnPropertyChanged("label");
            }
        }

    }
}
