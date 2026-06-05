using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBackground : MonoBehaviour
{
    [Header("References")]
    public Transform centerTarget;

    [Header("Space Background")]
    public bool setCameraBackground = true;
    public Color backgroundColor = new Color(0.005f, 0.005f, 0.025f);
    public Color ambientLightColor = new Color(0.08f, 0.08f, 0.12f);

    [Header("Stars")]
    public int starCount = 240;
    public float starFieldRadius = 35f;
    public float minStarSize = 0.02f;
    public float maxStarSize = 0.07f;
    public Color starColorA = new Color(0.65f, 0.8f, 1f);
    public Color starColorB = Color.white;
    public bool animateStars = true;

    [Header("Parallax Stars")]
    public bool createParallaxStars = true;
    public int parallaxStarCount = 80;
    public float parallaxRadius = 22f;
    public float parallaxMoveSpeed = 0.05f;

    [Header("Distant Planet")]
    public bool createPlanet = true;
    public Vector3 planetOffset = new Vector3(10f, 5f, 24f);
    public float planetSize = 7f;
    public Color planetColor = new Color(0.08f, 0.25f, 0.9f);
    public Color planetDarkSideColor = new Color(0.015f, 0.025f, 0.09f);
    public float planetRotationSpeed = 2f;

    [Header("Moon")]
    public bool createMoon = true;
    public Vector3 moonOffset = new Vector3(-9f, 6f, 26f);
    public float moonSize = 1.8f;
    public Color moonColor = new Color(0.65f, 0.68f, 0.75f);
    public float moonRotationSpeed = 0.8f;

    [Header("Shooting Stars")]
    public bool createShootingStars = true;
    public float shootingStarIntervalMin = 3.5f;
    public float shootingStarIntervalMax = 7f;
    public float shootingStarSpeed = 24f;
    public float shootingStarLength = 1.4f;
    public float shootingStarThickness = 0.045f;
    public Color shootingStarColor = new Color(0.75f, 0.95f, 1f);

    [Header("Asteroids")]
    public bool createAsteroids = true;
    public int asteroidCount = 18;
    public float asteroidRadius = 16f;
    public float asteroidMinSize = 0.12f;
    public float asteroidMaxSize = 0.35f;
    public float asteroidDriftSpeed = 0.25f;
    public Color asteroidColor = new Color(0.22f, 0.22f, 0.26f);

    [Header("Satellite")]
    public bool createSatellite = true;
    public float satelliteOrbitRadius = 9f;
    public float satelliteHeight = 4f;
    public float satelliteSpeed = 8f;
    public Color satelliteColor = new Color(0.55f, 0.65f, 0.8f);
    public Color satellitePanelColor = new Color(0.05f, 0.25f, 0.9f);

    private List<Transform> stars = new List<Transform>();
    private List<Vector3> starBaseScales = new List<Vector3>();
    private List<float> starOffsets = new List<float>();

    private List<Transform> parallaxStars = new List<Transform>();
    private List<Vector3> parallaxStarStartPositions = new List<Vector3>();

    private List<Transform> asteroids = new List<Transform>();
    private List<Vector3> asteroidStartPositions = new List<Vector3>();
    private List<Vector3> asteroidMoveDirections = new List<Vector3>();
    private List<float> asteroidOffsets = new List<float>();

    private Transform planetTransform;
    private Transform moonTransform;
    private Transform satelliteTransform;

    private Material starMaterial;
    private Material planetMaterial;
    private Material moonMaterial;
    private Material shootingStarMaterial;
    private Material asteroidMaterial;
    private Material satelliteMaterial;
    private Material satellitePanelMaterial;

    private float shootingStarTimer;
    private float nextShootingStarTime;

    private void Start()
    {
        SetupEnvironment();
        CreateMaterials();
        CreateStars();

        if (createParallaxStars)
        {
            CreateParallaxStars();
        }

        if (createPlanet)
        {
            CreatePlanet();
        }

        if (createMoon)
        {
            CreateMoon();
        }

        if (createAsteroids)
        {
            CreateAsteroids();
        }

        if (createSatellite)
        {
            CreateSatellite();
        }

        ResetShootingStarTimer();
    }

    private void Update()
    {
        if (animateStars)
        {
            AnimateStars();
        }

        AnimateParallaxStars();
        AnimatePlanetAndMoon();
        AnimateAsteroids();
        AnimateSatellite();
        HandleShootingStars();
    }

    private void SetupEnvironment()
    {
        if (setCameraBackground)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = backgroundColor;
            }
        }

        RenderSettings.fog = false;
        RenderSettings.ambientLight = ambientLightColor;
    }

    private void CreateMaterials()
    {
        starMaterial = CreateUnlitMaterial(Color.white);
        planetMaterial = CreatePlanetMaterial(planetColor);
        moonMaterial = CreatePlanetMaterial(moonColor);
        shootingStarMaterial = CreateUnlitMaterial(shootingStarColor);
        asteroidMaterial = CreatePlanetMaterial(asteroidColor);
        satelliteMaterial = CreatePlanetMaterial(satelliteColor);
        satellitePanelMaterial = CreateUnlitMaterial(satellitePanelColor);
    }

    private Material CreateUnlitMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        Material material = new Material(shader);
        material.color = color;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 1.2f);
        }

        return material;
    }

    private Material CreatePlanetMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            return CreateUnlitMaterial(color);
        }

        Material material = new Material(shader);

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else
        {
            material.color = color;
        }

        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", 0.05f);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.55f);
        }

        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 0.08f);
        }

        return material;
    }

    private void CreateStars()
    {
        GameObject starParent = new GameObject("Generated_Stars");
        starParent.transform.SetParent(transform);

        Vector3 center = GetCenterPosition();

        for (int i = 0; i < starCount; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            Vector3 position = center + direction * Random.Range(starFieldRadius * 0.65f, starFieldRadius);

            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "Star_" + i;
            star.transform.SetParent(starParent.transform);
            star.transform.position = position;

            float size = Random.Range(minStarSize, maxStarSize);
            star.transform.localScale = Vector3.one * size;

            Renderer renderer = star.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material = new Material(starMaterial);

                Color chosenColor = Color.Lerp(
                    starColorA,
                    starColorB,
                    Random.Range(0f, 1f)
                );

                material.color = chosenColor;

                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", chosenColor);
                }

                if (material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", chosenColor * 1.3f);
                }

                renderer.material = material;
            }

            Collider collider = star.GetComponent<Collider>();

            if (collider != null)
            {
                Destroy(collider);
            }

            stars.Add(star.transform);
            starBaseScales.Add(star.transform.localScale);
            starOffsets.Add(Random.Range(0f, 100f));
        }
    }

    private void CreateParallaxStars()
    {
        GameObject parallaxParent = new GameObject("Generated_ParallaxStars");
        parallaxParent.transform.SetParent(transform);

        Vector3 center = GetCenterPosition();

        for (int i = 0; i < parallaxStarCount; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            Vector3 position = center + direction * Random.Range(parallaxRadius * 0.7f, parallaxRadius);

            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "ParallaxStar_" + i;
            star.transform.SetParent(parallaxParent.transform);
            star.transform.position = position;

            float size = Random.Range(minStarSize * 1.4f, maxStarSize * 1.8f);
            star.transform.localScale = Vector3.one * size;

            Renderer renderer = star.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material = new Material(starMaterial);
            }

            Collider collider = star.GetComponent<Collider>();

            if (collider != null)
            {
                Destroy(collider);
            }

            parallaxStars.Add(star.transform);
            parallaxStarStartPositions.Add(position);
        }
    }

    private void CreatePlanet()
    {
        Vector3 center = GetCenterPosition();

        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.name = "Generated_DistantPlanet";
        planet.transform.SetParent(transform);
        planet.transform.position = center + planetOffset;
        planet.transform.localScale = Vector3.one * planetSize;

        Renderer renderer = planet.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = planetMaterial;
        }

        Collider collider = planet.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }

        GameObject shadow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shadow.name = "Generated_PlanetDarkSide";
        shadow.transform.SetParent(planet.transform);
        shadow.transform.localPosition = new Vector3(-0.18f, -0.04f, -0.08f);
        shadow.transform.localScale = Vector3.one * 1.01f;

        Renderer shadowRenderer = shadow.GetComponent<Renderer>();

        if (shadowRenderer != null)
        {
            Material shadowMaterial = CreateUnlitMaterial(planetDarkSideColor);
            shadowRenderer.material = shadowMaterial;
        }

        Collider shadowCollider = shadow.GetComponent<Collider>();

        if (shadowCollider != null)
        {
            Destroy(shadowCollider);
        }

        planetTransform = planet.transform;
    }

    private void CreateMoon()
    {
        Vector3 center = GetCenterPosition();

        GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        moon.name = "Generated_Moon";
        moon.transform.SetParent(transform);
        moon.transform.position = center + moonOffset;
        moon.transform.localScale = Vector3.one * moonSize;

        Renderer renderer = moon.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = moonMaterial;
        }

        Collider collider = moon.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }

        moonTransform = moon.transform;
    }

    private void CreateAsteroids()
    {
        GameObject asteroidParent = new GameObject("Generated_Asteroids");
        asteroidParent.transform.SetParent(transform);

        Vector3 center = GetCenterPosition();

        for (int i = 0; i < asteroidCount; i++)
        {
            Vector3 randomPosition = center + Random.onUnitSphere * Random.Range(asteroidRadius * 0.6f, asteroidRadius);

            if (randomPosition.y > -1f && randomPosition.y < 3f)
            {
                randomPosition.y += Random.value > 0.5f ? 4f : -4f;
            }

            GameObject asteroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            asteroid.name = "Asteroid_" + i;
            asteroid.transform.SetParent(asteroidParent.transform);
            asteroid.transform.position = randomPosition;

            float size = Random.Range(asteroidMinSize, asteroidMaxSize);
            asteroid.transform.localScale = new Vector3(
                size * Random.Range(0.7f, 1.3f),
                size * Random.Range(0.6f, 1.1f),
                size * Random.Range(0.7f, 1.4f)
            );

            asteroid.transform.rotation = Random.rotation;

            Renderer renderer = asteroid.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material = asteroidMaterial;
            }

            Collider collider = asteroid.GetComponent<Collider>();

            if (collider != null)
            {
                Destroy(collider);
            }

            Vector3 moveDirection = new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.4f, 0.4f)
            ).normalized;

            if (moveDirection.sqrMagnitude < 0.1f)
            {
                moveDirection = Vector3.right;
            }

            asteroids.Add(asteroid.transform);
            asteroidStartPositions.Add(randomPosition);
            asteroidMoveDirections.Add(moveDirection);
            asteroidOffsets.Add(Random.Range(0f, 100f));
        }
    }

    private void CreateSatellite()
    {
        GameObject satellite = new GameObject("Generated_Satellite");
        satellite.transform.SetParent(transform);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Satellite_Body";
        body.transform.SetParent(satellite.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.35f, 0.25f, 0.25f);

        Renderer bodyRenderer = body.GetComponent<Renderer>();
        if (bodyRenderer != null)
        {
            bodyRenderer.material = satelliteMaterial;
        }

        Collider bodyCollider = body.GetComponent<Collider>();
        if (bodyCollider != null)
        {
            Destroy(bodyCollider);
        }

        CreateSatellitePanel(satellite.transform, new Vector3(-0.55f, 0f, 0f));
        CreateSatellitePanel(satellite.transform, new Vector3(0.55f, 0f, 0f));

        satelliteTransform = satellite.transform;
    }

    private void CreateSatellitePanel(Transform parent, Vector3 localPosition)
    {
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "Satellite_Panel";
        panel.transform.SetParent(parent);
        panel.transform.localPosition = localPosition;
        panel.transform.localScale = new Vector3(0.45f, 0.04f, 0.28f);

        Renderer renderer = panel.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = satellitePanelMaterial;
        }

        Collider collider = panel.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }
    }

    private void AnimateStars()
    {
        for (int i = 0; i < stars.Count; i++)
        {
            Transform star = stars[i];

            if (star == null)
            {
                continue;
            }

            float twinkle = 1f + Mathf.Sin(Time.time * 1.4f + starOffsets[i]) * 0.1f;
            star.localScale = starBaseScales[i] * twinkle;
        }
    }

    private void AnimateParallaxStars()
    {
        if (!createParallaxStars)
        {
            return;
        }

        for (int i = 0; i < parallaxStars.Count; i++)
        {
            Transform star = parallaxStars[i];

            if (star == null)
            {
                continue;
            }

            Vector3 startPosition = parallaxStarStartPositions[i];

            float moveX = Mathf.Sin(Time.time * parallaxMoveSpeed + i) * 0.25f;
            float moveY = Mathf.Cos(Time.time * parallaxMoveSpeed * 0.6f + i) * 0.15f;

            star.position = startPosition + new Vector3(moveX, moveY, 0f);
        }
    }

    private void AnimatePlanetAndMoon()
    {
        if (planetTransform != null)
        {
            planetTransform.Rotate(Vector3.up, planetRotationSpeed * Time.deltaTime, Space.World);
        }

        if (moonTransform != null)
        {
            moonTransform.Rotate(Vector3.up, moonRotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void AnimateAsteroids()
    {
        if (!createAsteroids)
        {
            return;
        }

        for (int i = 0; i < asteroids.Count; i++)
        {
            Transform asteroid = asteroids[i];

            if (asteroid == null)
            {
                continue;
            }

            float offset = asteroidOffsets[i];

            Vector3 drift = asteroidMoveDirections[i] * Mathf.Sin(Time.time * asteroidDriftSpeed + offset) * 0.8f;
            asteroid.position = asteroidStartPositions[i] + drift;

            asteroid.Rotate(
                new Vector3(15f, 25f, 10f),
                Time.deltaTime * 4f,
                Space.Self
            );
        }
    }

    private void AnimateSatellite()
    {
        if (!createSatellite || satelliteTransform == null)
        {
            return;
        }

        Vector3 center = GetCenterPosition();

        float angle = Time.time * satelliteSpeed * Mathf.Deg2Rad;

        Vector3 position = center + new Vector3(
            Mathf.Cos(angle) * satelliteOrbitRadius,
            satelliteHeight + Mathf.Sin(angle * 0.7f) * 0.6f,
            Mathf.Sin(angle) * satelliteOrbitRadius
        );

        satelliteTransform.position = position;
        satelliteTransform.LookAt(center);
        satelliteTransform.Rotate(Vector3.up, 90f);
    }

    private void HandleShootingStars()
    {
        if (!createShootingStars)
        {
            return;
        }

        shootingStarTimer += Time.deltaTime;

        if (shootingStarTimer >= nextShootingStarTime)
        {
            SpawnShootingStar();
            ResetShootingStarTimer();
        }
    }

    private void ResetShootingStarTimer()
    {
        shootingStarTimer = 0f;
        nextShootingStarTime = Random.Range(shootingStarIntervalMin, shootingStarIntervalMax);
    }

    private void SpawnShootingStar()
    {
        Vector3 center = GetCenterPosition();

        GameObject shootingStar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shootingStar.name = "Generated_ShootingStar";
        shootingStar.transform.SetParent(transform);

        Vector3 startPosition = center + new Vector3(
            Random.Range(-13f, 13f),
            Random.Range(4f, 10f),
            Random.Range(18f, 30f)
        );

        Vector3 direction = new Vector3(
            Random.Range(-1f, -0.35f),
            Random.Range(-0.35f, -0.08f),
            Random.Range(-0.1f, 0.1f)
        ).normalized;

        shootingStar.transform.position = startPosition;
        shootingStar.transform.localScale = new Vector3(
            shootingStarLength,
            shootingStarThickness,
            shootingStarThickness
        );

        shootingStar.transform.rotation = Quaternion.LookRotation(direction);

        Renderer renderer = shootingStar.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = new Material(shootingStarMaterial);
        }

        Collider collider = shootingStar.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }

        StartCoroutine(MoveAndDestroyShootingStar(shootingStar.transform, direction));
    }

    private IEnumerator MoveAndDestroyShootingStar(Transform shootingStar, Vector3 direction)
    {
        float lifetime = 1.15f;
        float timer = 0f;

        Vector3 startScale = shootingStar.localScale;

        while (shootingStar != null && timer < lifetime)
        {
            timer += Time.deltaTime;

            shootingStar.position += direction * shootingStarSpeed * Time.deltaTime;

            float fade = 1f - timer / lifetime;

            shootingStar.localScale = new Vector3(
                startScale.x * fade,
                startScale.y * fade,
                startScale.z * fade
            );

            yield return null;
        }

        if (shootingStar != null)
        {
            Destroy(shootingStar.gameObject);
        }
    }

    private Vector3 GetCenterPosition()
    {
        if (centerTarget != null)
        {
            return centerTarget.position;
        }

        return Vector3.zero;
    }
}