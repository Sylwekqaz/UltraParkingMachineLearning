using Ultra.Contract.Model;

namespace Ultra.Contract
{
    public interface IClassifier
    {
        bool Predict(ImageFeatures imageFeatures);
    }
}