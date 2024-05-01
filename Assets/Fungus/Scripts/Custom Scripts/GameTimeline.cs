using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class GameTimeline : MonoBehaviour
{
    public string sceneName;
    public VideoPlayer videoPlayer = null;
    private float gameTimer = 0;

    private void Awake()
    {
        /*
        Establece el vSyncCount de la dependencia UnityEngine.QualitySettings 
        a 1 para asegura que el VideoPlayer esté sincronizado con el frame rate
        */
        QualitySettings.vSyncCount = 1;

        //Inicia un gameTimer almacenando el timer actual
        gameTimer = Time.time;

        //Llama a la corrutina StartCoroutine(PlayVideo()) de la libreria UnityEngine.MonoBehaviour para reproducir el vídeo
        StartCoroutine(PlayVideo());
    }

    //La corrutina PlayVideo() es la responsable de reproducir el vídeo
    IEnumerator PlayVideo()
    {
        //Prepara el video player
        bool videoLoadedFailed = false;
        videoPlayer.Prepare();

        //Espera a que el video player esté preparado para reproducir el vídeo
        while (!videoPlayer.isPrepared)
        {
            //Si el vídeo tarda más de 11 segundos en prepararse
            if (Time.time > gameTimer + 11.0f)
            {
                //Se asume que el vídeo ha fallado en cargarse, saltamos el código del video player para cargar directamente la escena
                videoLoadedFailed = true;
                break;
            }

            //Devolvemos null para evitar crash
            yield return null;
        }

        //Si la carga del video no ha fallado
        if (!videoLoadedFailed)
        {
            //Iniciamos la carga del vídeo
            videoPlayer.Play();
        }


        gameTimer = Time.time;
        //Mientras se reproduce el vídeo
        while (videoPlayer.isPlaying)
        {
            //Comprobamos que se esté repoduciendo siendo el tiempo actual mayor al al guardado en la variable
            if (Time.time > gameTimer + 0.6f)
            {
                gameTimer = Time.time;
            }

            //Si se recibe un innput como las teclas Esc o Espacio
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                //Se detiente la reproducción
                videoPlayer.Stop();
                break;
            }

            yield return null;
        }

        //Desactivamos el vSyncCount
        QualitySettings.vSyncCount = 0;
        //Empieza a cargar la escena de forma asíncrona
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(sceneName);
        //Inicialmente desactiva la carga de la escena mientras se cargan los datos de forma asíncrona
        sceneLoader.allowSceneActivation = false;
        // Esperar a que la escena se cargue completamente
        while (!sceneLoader.isDone)
        {
            if (sceneLoader.progress >= 0.9f)
            {
                // Los assets de la escena se han cargado casi por completo
                // Puedes activar la escena en este punto
                sceneLoader.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

