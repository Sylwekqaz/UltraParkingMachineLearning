using Logic.Model;

namespace Logic.Classifiers
{
    public interface IClassifier
    {
        int Predict(ImageFeatures imageFeatures);
    }
}