using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class GizmosColor : IDisposable
{
    private static Color _orgColor;

    public GizmosColor(Color newColor)
    {
        _orgColor = Gizmos.color;
        Gizmos.color = newColor;
    }

    public void Dispose()
    {
        Gizmos.color = _orgColor;
    }
}

public class AxisDrawer : MonoBehaviour
{
    private const float LINE_LENGTH = 5f;

    [SerializeField] private float real = 1;
    [SerializeField] private float imaginary = 1;
    [SerializeField] private Text posLabel;
    
    private Complex complexNum = Complex.One;
    private float degree = 45;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        try
        {
            complexNum = new Complex(real, imaginary);
            using (new GizmosColor(Color.red))
            {
                Gizmos.DrawLine(Vector3.zero, Vector3.right * LINE_LENGTH);
                Gizmos.DrawLine(Vector3.zero, Vector3.left * LINE_LENGTH);
            }

            using (new GizmosColor(Color.green))
            {
                Gizmos.DrawLine(Vector3.zero, Vector3.up * LINE_LENGTH);
                Gizmos.DrawLine(Vector3.zero, Vector3.down * LINE_LENGTH);
            }

            using (new GizmosColor(Color.yellow))
            {
                var lineEndPos = new Vector3(real, imaginary);
                Gizmos.DrawLine(Vector3.zero, lineEndPos);

                var labelPos = lineEndPos + lineEndPos.normalized * 2 ;
                if (posLabel is not null)
                {
                    posLabel.text = $"({complexNum.Real:F3}, {complexNum.Imaginary:F3})\n{complexNum.Real:F3} + {complexNum.Imaginary:F3}i";
                    posLabel.transform.position = labelPos + Vector3.back;
                }
            }
        }
        finally
        {
            using (new GizmosColor(Color.red))
            {
                Gizmos.DrawSphere(Vector3.zero, .2f);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Window(0, new Rect(0, 0, 500, 100),
            _ =>
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("How much do you want to rotate? (In degree)");
                    var textInput = GUILayout.TextField(degree.ToString());
                    float.TryParse(textInput, out degree);
                }
                
                var radAngle = degree * Mathf.Deg2Rad;
                var rotationComplexNum = new Complex(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label($"That'll give us a complex number of cos({degree}\u00b0) * sin({degree}\u00b0)i");
                    if (GUILayout.Button("Rotate"))
                    {
                        complexNum *= rotationComplexNum;
                        real = (float)complexNum.Real;
                        imaginary = (float)complexNum.Imaginary;
                    }
                }
                GUILayout.Label($"When the button is pressed, an operation of ({complexNum.ToComplexNumString()}) * ({rotationComplexNum.ToComplexNumString()}) will be executed, which will rotate the line shown on the screen.");
            }, "User Input");
    }
}

public static class Helper
{
    public static string ToComplexNumString(this Complex num)
    {
        return $"{num.Real:F3} + {num.Imaginary:F3}i";
    }
}