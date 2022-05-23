using System.Collections.Generic;
using UnityEngine;

public class ShellPool : MonoBehaviour
{
    public static ShellPool Instance { get; private set; }

    [Header("Prefabs")] 
    [SerializeField] private Shell pistolShellPrefab;
    [SerializeField] private Shell shotgunShellPrefab;
    [SerializeField] private Shell rifleShellPrefab;

    [Header("Pools")] 
    [SerializeField] private Transform pistolShellPool;
    [SerializeField] private Transform shotgunShellPool;
    [SerializeField] private Transform rifleShellPool;

    private Queue<Shell> _pistolShells = new();
    private Queue<Shell> _shotgunShells = new();
    private Queue<Shell> _rifleShells = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GeneratePools();
    }

    private void GeneratePools()
    {
        AddShells(50, ShellType.PISTOL);
        AddShells(50, ShellType.SHOTGUN);
        AddShells(50, ShellType.RIFLE);
    }

    private void AddShells(int count, ShellType shellType)
    {
        switch (shellType)
        {
            case ShellType.PISTOL:
                for (int i = 0; i < count; i++)
                {
                    var shell = Instantiate(pistolShellPrefab, pistolShellPool);
                    shell.SetShellType(shellType);
                    shell.gameObject.SetActive(false);
                    _pistolShells.Enqueue(shell);
                }
                break;
            case ShellType.SHOTGUN:
                for (int i = 0; i < count; i++)
                {
                    var shell = Instantiate(shotgunShellPrefab, shotgunShellPool);
                    shell.SetShellType(shellType);
                    shell.gameObject.SetActive(false);
                    _shotgunShells.Enqueue(shell);
                }
                break;
            case ShellType.RIFLE:
                for (int i = 0; i < count; i++)
                {
                    var shell = Instantiate(rifleShellPrefab, rifleShellPool);
                    shell.SetShellType(shellType);
                    shell.gameObject.SetActive(false);
                    _rifleShells.Enqueue(shell);
                }
                break;
        }
    }

    public Shell Get(ShellType shellType)
    {
        switch (shellType)
        {
            case ShellType.PISTOL:
                if (_pistolShells.Count == 0) AddShells(1, shellType);
                return _pistolShells.Dequeue();
            case ShellType.SHOTGUN:
                if (_shotgunShells.Count == 0) AddShells(1, shellType);
                return _shotgunShells.Dequeue();
            case ShellType.RIFLE:
                if (_rifleShells.Count == 0) AddShells(1, shellType);
                return _rifleShells.Dequeue();
        }

        return null;
    }

    public void ReturnToPool(Shell shell)
    {
        shell.gameObject.SetActive(false);

        switch (shell.GetShellType)
        {
            case ShellType.PISTOL:
                _pistolShells.Enqueue(shell);
                break;
            case ShellType.SHOTGUN:
                _shotgunShells.Enqueue(shell);
                break;
            case ShellType.RIFLE:
                _rifleShells.Enqueue(shell);
                break;
        }
    }
}