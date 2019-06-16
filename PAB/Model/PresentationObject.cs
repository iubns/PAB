using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using PAB.Objects;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PAB.Model
{
    public class PresnetationObject
    {
        Slides slides;
        _Slide slide;
        TextRange objText;

        Microsoft.Office.Interop.PowerPoint.Application pptApplication;
        Presentation pptPresentation;
        readonly CustomLayout customLayout;

        int fontSize;
        string fontName;
        string bakcgoundFilePath;

        public PresnetationObject()
        {
            pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();
            pptPresentation = pptApplication.Presentations.Add(MsoTriState.msoTrue);
            customLayout = pptPresentation.SlideMaster.CustomLayouts[PpSlideLayout.ppLayoutChartAndText];
        }

        public void CreatePowerPointSlides(Setting setting)
        {
            fontSize = int.Parse(setting.fontSize);
            fontName = setting.fontName;
            bakcgoundFilePath = setting.backgoundFilePath;
            
            ObservableCollection <ShowingObject> list = ListManager.GetMakeList();
            foreach (ShowingObject showingObject in list.Reverse())
            {
                slide = MakeSlide("");

                string content = showingObject.content;
                string[] contents = content.Split(new string[] { "\n\n" }, StringSplitOptions.None);
                for (int i = 0; i < contents.Length; i++)
                {
                    slide = MakeSlide(contents[(contents.Length - 1) - i]);
                }
            }
        }

        private _Slide MakeSlide(string lyrics)
        {
            slides = pptPresentation.Slides;
            slide = slides.AddSlide(1, customLayout);
            slide.BackgroundStyle = MsoBackgroundStyleIndex.msoBackgroundStylePreset4;

            slide.Shapes[1].Width = 32 * 30;
            slide.Shapes[1].Height = 18 * 30;
            slide.Shapes[1].Top = 0;
            slide.Shapes[1].Left = 0;

            objText = slide.Shapes[1].TextFrame.TextRange;
            objText.Text = lyrics;
            objText.ParagraphFormat.Alignment = PpParagraphAlignment.ppAlignCenter;
            objText.Font.NameFarEast = fontName;
            objText.Font.Name = fontName;
            objText.Font.Size = fontSize;
            objText.Font.Bold = MsoTriState.msoTrue;
            objText.Font.Color.RGB = 16777215;

            if (bakcgoundFilePath != "파일 선택")
            {
                try {
                    slide.Shapes.AddPicture2(bakcgoundFilePath, MsoTriState.msoFalse, MsoTriState.msoCTrue, 0, 0, 32 * 30, 18 * 30);
                    slide.Shapes[2].ZOrder(MsoZOrderCmd.msoSendBackward);
                }
                catch
                {
                    MessageBox.Show("배경 파일 경로 오류");
                }
            }
            return slide;
        }
    }
}
