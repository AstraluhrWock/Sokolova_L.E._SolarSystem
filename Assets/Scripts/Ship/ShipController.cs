using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkMovableObject
{ 
    [SerializeField] private Transform _cameraAttach;
    private CameraOrbit _cameraOrbit;
    private PlayerLabel _playerLabel;
    private float _shipSpeed;
    private Rigidbody _rb;
    private StarCollector _starCollector;
    
    [SyncVar] private string _playerName;

    protected override float _speed => _shipSpeed;
    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }

    private void OnGUI()
    {
        if (_cameraOrbit == null)
        {
            return;
        }
        _cameraOrbit.ShowPlayerLabels(_playerLabel);
    }
    public override void OnStartAuthority()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            return;
        }
        gameObject.name = _playerName;
        _cameraOrbit = FindObjectOfType<CameraOrbit>();
        _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
        _playerLabel = GetComponentInChildren<PlayerLabel>();
       _starCollector = FindObjectOfType<StarCollector>();
        base.OnStartAuthority();
    }

    private void OnTriggerEnter(Collider other)
    {    
        if (other.gameObject.GetComponent<Star>())
        {
           _starCollector.CmdCollectStar();
            other.gameObject.SetActive(false);
            Debug.Log("Star collected");
        }
        else
        {   
            gameObject.SetActive(false);
            gameObject.transform.position = new Vector3(100, 100, 100);
            gameObject.SetActive(true);
        }
    }
    protected override void HasAuthorityMovement()
    {
        var spaceShipSettings =
        SettingsContainer.Instance?.SpaceShipSettings;
        if (spaceShipSettings == null)
        {
            return;
        }
        var isFaster = Input.GetKey(KeyCode.LeftShift);
        var speed = spaceShipSettings.ShipSpeed;
        var faster = isFaster ? spaceShipSettings.Faster : 1.0f;
        _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, SettingsContainer.Instance.SpaceShipSettings.Acceleration);
        var currentFov = isFaster ? SettingsContainer.Instance.SpaceShipSettings.FasterFov : SettingsContainer.Instance.SpaceShipSettings.NormalFov;
        _cameraOrbit.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);
        var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
        _rb.velocity = velocity * Time.deltaTime;
        if (!Input.GetKey(KeyCode.C))
        {
            var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }
    protected override void FromServerUpdate() { }
    protected override void SendToServer() { }
    [ClientCallback]
    private void LateUpdate()
    {
        _cameraOrbit?.CameraMovement();
    }
}
