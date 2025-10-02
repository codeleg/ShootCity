using UnityEngine;
using UnityEngine.UI; // Crosshair için renk değiştirme

public class GunAim : MonoBehaviour
{
    [Header("Gun Ucunu Mouse ile Kontrol")]
    public Transform gunEnd;
    public float mouseSensitivityX = 2f;
    public float mouseSensitivityY = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("UI Crosshair")]
    public RectTransform crosshairUI; // UI Crosshair referansı
    private Image crosshairImage;     // Crosshair'in Image bileşeni (renk değiştirmek için)

    private Camera _camPlayer;

    void Start()
    {
        // Fareyi gizle ve ortada kilitle
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angles = transform.localEulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
        _camPlayer = Camera.main; // cacheledik

        if (crosshairUI != null)
            crosshairImage = crosshairUI.GetComponent<Image>();
    }

    void Update()
    {
        if (gunEnd == null || _camPlayer == null) return;

        // TPS hissi için rotasyon
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivityX;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        rotationY = Mathf.Clamp(rotationY, -80f, 80f);
        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);

        // Crosshair'i daima ekran ortasında tut
        if (crosshairUI != null)
            crosshairUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Kamera merkezinden ray → hedef kontrolü
        Ray camRay = _camPlayer.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPos = camRay.GetPoint(100f); // default hedef

        if (Physics.Raycast(camRay, out RaycastHit hit, 1000f)) // bu blogu kaldırcam buyük ihtimal sadece renk değitşiryor
        {
            targetPos = hit.point;

            // Vurulabilir bir şey buldu → crosshair kırmızı
            if (crosshairImage != null)
                crosshairImage.color = Color.red;
        }
        else
        {
            // Hiçbir şeye çarpmadı → crosshair beyaz
            if (crosshairImage != null)
                crosshairImage.color = Color.white;
        }

        // GunEnd'i hedefe yönlendir
        gunEnd.rotation = Quaternion.LookRotation((targetPos - gunEnd.position).normalized);
    }
}
