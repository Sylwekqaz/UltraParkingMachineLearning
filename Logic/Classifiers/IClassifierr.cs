using Logic.Model;

namespace Logic.Classifiers
{
    public interface IClassifier
    {
        bool Predict(ImageFeatures imageFeatures);
    }
}