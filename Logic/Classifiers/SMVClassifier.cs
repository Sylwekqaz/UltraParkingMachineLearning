using System.Collections.Generic;
using Logic.Model;
using Logic.utils;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace Logic.Classifiers
{

    public class SMVClassifier : IClassifier
    {
        private readonly SVM _svm;

        private SMVClassifier(SVM svm)
        {
            _svm = svm;
        }

        public int Predict(ImageFeatures imageFeatures)
        {
            return (int) _svm.Predict(imageFeatures.ToPredictionMat());
        }


        public static SMVClassifier Create(List<ImageFeatures> trainingData)
        {
            var svm = SVM.Create();
            svm.Type = SVM.Types.CSvc;
            svm.KernelType = SVM.KernelTypes.Rbf;
            svm.TermCriteria = TermCriteria.Both(1000, 0.000001);
            svm.Degree = 100.0;
            svm.Gamma = 100.0;
            svm.Coef0 = 1.0;
            svm.C = 1.0;
            svm.Nu = 0.5;
            svm.P = 0.1;

            svm.Train(trainingData.ToTrainingMat(), SampleTypes.RowSample, trainingData.ToResponseMat());

            return new SMVClassifier(svm);
        }
    }
}