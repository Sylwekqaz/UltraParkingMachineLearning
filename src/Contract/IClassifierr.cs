using Contract.Model;

namespace Contract
{
    public interface IClassifier
    {
        bool Predict(ImageFeatures imageFeatures);
    }
}