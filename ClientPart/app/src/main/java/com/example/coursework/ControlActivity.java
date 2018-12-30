package com.example.coursework;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.util.UUID;

import AdditionalClasses.Commands;
import ru.yandex.speechkit.Error;
import ru.yandex.speechkit.Language;
import ru.yandex.speechkit.OnlineModel;
import ru.yandex.speechkit.OnlineRecognizer;
import ru.yandex.speechkit.PhraseSpotter;
import ru.yandex.speechkit.PhraseSpotterListener;
import ru.yandex.speechkit.Recognition;
import ru.yandex.speechkit.Recognizer;
import ru.yandex.speechkit.RecognizerListener;
import ru.yandex.speechkit.SpeechKit;
import ru.yandex.speechkit.Track;

import static android.Manifest.permission.RECORD_AUDIO;
import static android.content.pm.PackageManager.PERMISSION_GRANTED;

public class ControlActivity extends AppCompatActivity implements RecognizerListener, PhraseSpotterListener {
    private static final String API_KEY = "";
    private static final int REQUEST_PERMISSION_CODE = 31;
    private String ipAddress;
    private int port = 10000;
    private byte command;
    private int bufferSize = 8;
    private byte[] totalBytes;
    private boolean isSendingMust;
    private OnlineRecognizer recognizer;
    private PhraseSpotter phraseSpotter;
    private TextView info;
    private String audioCommand;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        try {
            SpeechKit.getInstance().init(getApplicationContext(), API_KEY);
            SpeechKit.getInstance().setUuid(UUID.randomUUID().toString());
            SpeechKit.getInstance().setLogLevel(SpeechKit.LogLevel.LOG_DEBUG);
        } catch (ru.yandex.speechkit.SpeechKit.LibraryInitializationException e) {
            Toast toast = Toast.makeText(this, e.getMessage(), Toast.LENGTH_LONG);
            toast.show();
        }

        recognizer = new OnlineRecognizer.Builder(Language.RUSSIAN, OnlineModel.QUERIES, this)
                .setDisableAntimat(false)
                .setEnablePunctuation(true)
                .build();
        recognizer.prepare();

        phraseSpotter = new PhraseSpotter.Builder("phrase-spotter/commands", this).build();
        phraseSpotter.prepare();

        Bundle arguments = getIntent().getExtras();
        ipAddress = arguments.get("IP").toString();

        setContentView(R.layout.activity_control);

        info = findViewById(R.id.SpeechInfo);
        RunPhraseSpotter();
        UpdateInfo(String.valueOf(phraseSpotter.isLoggingEnabled()));
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.settings, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == R.id.GetImage) {
            if(!item.isChecked()) item.setChecked(true);
            else {
                item.setChecked(false);
                ImageView image = findViewById(R.id.Screenshot);
                image.setImageResource(0);
            }
            isSendingMust = item.isChecked();
        }

        return super.onOptionsItemSelected(item);
    }

    public void onClick (View view) {
        switch (view.getId())
        {
            case R.id.StartPresentation:
            {
                command = Commands.startPresentation;
                new SenderThread().execute();
                break;
            }
            case R.id.EndPresentation:
            {
                command = Commands.stopPresentation;
                new SenderThread().execute();
                break;
            }
            case R.id.NextSlide:
            {
                command = Commands.nextSlide;
                new SenderThread().execute();
                break;
            }
            case R.id.PreviousSlide:
            {
                command = Commands.previousSlide;
                new SenderThread().execute();
                break;
            }
            case R.id.ControlPanel:
            {
                Intent intent = new Intent(this, ControlPanel.class);
                intent.putExtra("IP", ipAddress);
                startActivity(intent);
                break;
            }
        }
    }

    private void StartRecording() {
        if (ContextCompat.checkSelfPermission(this, RECORD_AUDIO) != PERMISSION_GRANTED)
        {
            ActivityCompat.requestPermissions(this, new String[]{RECORD_AUDIO}, REQUEST_PERMISSION_CODE);
        }
        else {
            recognizer.startRecording();
        }
    }

    private void RunPhraseSpotter() {
        if (ContextCompat.checkSelfPermission(this, RECORD_AUDIO) != PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{RECORD_AUDIO}, REQUEST_PERMISSION_CODE);
        } else {
            phraseSpotter.start();
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           @NonNull String[] permissions,
                                           @NonNull int[] grantResults) {
        if (requestCode != REQUEST_PERMISSION_CODE) {
            super.onRequestPermissionsResult(requestCode, permissions, grantResults);
            return;
        }

        if (grantResults.length == 1 && grantResults[0] == PERMISSION_GRANTED) {
            RunPhraseSpotter();
        } else {
            UpdateInfo("Record audio permission was not granted");
        }
    }

    private void UpdateInfo(String text)
    {
        info.setText(text);
    }

    @Override
    public void onRecordingBegin(@NonNull Recognizer recognizer) {
        UpdateInfo("Recording");
    }

    @Override
    public void onSpeechDetected(@NonNull Recognizer recognizer) {

    }

    @Override
    public void onSpeechEnds(@NonNull Recognizer recognizer) {
    }

    @Override
    public void onRecordingDone(@NonNull Recognizer recognizer) {
    }

    @Override
    public void onPowerUpdated(@NonNull Recognizer recognizer, float v) {

    }

    @Override
    public void onPartialResults(@NonNull Recognizer recognizer, @NonNull Recognition recognition, boolean b) {
        if (b)
        {
            String temp = recognition.getBestResultText();
            audioCommand = temp.substring(0, temp.length()-1);
        }
    }

    @Override
    public void onRecognitionDone(@NonNull Recognizer recognizer) {
        UpdateInfo(audioCommand);
        switch (audioCommand) {
            case Commands.StartPresentation:{
                command = Commands.startPresentation;
                new SenderThread().execute();
                break;
            }
            case Commands.EndPresentation: {
                command = Commands.stopPresentation;
                new SenderThread().execute();
                break;
            }
            case Commands.NextSlide: {
                command = Commands.nextSlide;
                new SenderThread().execute();
                break;
            }
            case Commands.PreviousSlide: {
                command = Commands.previousSlide;
                new SenderThread().execute();
                break;
            }
            case Commands.ControlPanel: {
                Intent intent = new Intent(this, ControlPanel.class);
                intent.putExtra("IP", ipAddress);
                startActivity(intent);
                break;
            }
        }
        recognizer.stopRecording();
    }

    @Override
    public void onRecognizerError(@NonNull Recognizer recognizer, @NonNull Error error) {
        UpdateInfo("Recognizer error: " + error);
        recognizer.stopRecording();
    }

    @Override
    public void onMusicResults(@NonNull Recognizer recognizer, @NonNull Track track) {

    }

    @Override
    public void onPhraseSpotted(@NonNull PhraseSpotter phraseSpotter, @NonNull String s, int i) {
        if(s.equals(Commands.VoiceActivator))
        {
            StartRecording();
        }
    }

    @Override
    public void onPhraseSpotterStarted(@NonNull PhraseSpotter phraseSpotter) {
        UpdateInfo("Valera is running");
    }

    @Override
    public void onPhraseSpotterError(@NonNull PhraseSpotter phraseSpotter, @NonNull Error error) {

    }

    class SenderThread extends AsyncTask<Void, Void, Void> {
        @Override
        protected Void doInBackground(Void... params) {
            try {
                Socket socket = null;
                try {
                    InetAddress ip = InetAddress.getByName(ipAddress);
                    socket = new Socket(ip, port);

                    OutputStream outputStream = socket.getOutputStream();
                    DataOutputStream out = new DataOutputStream(outputStream);
                    DataInputStream in = new DataInputStream(socket.getInputStream());

                    byte[] parcel = new byte[2];
                    if(isSendingMust)
                    {
                        parcel[0] = 1;
                        parcel[1] = command;
                        out.write(parcel);

                        InputStreamReader inputStreamReader = new InputStreamReader(socket.getInputStream());
                        BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
                        int receivedBytesLength = Integer.parseInt(bufferedReader.readLine());
                        totalBytes = new byte[receivedBytesLength];
                        try {
                            Thread.sleep(50);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                        int currentPosition = 0;
                        byte[] receivedBytes = new byte[bufferSize];
                        while ((in.read(receivedBytes)) > -1) {
                            for (int i = currentPosition, j = 0; j < receivedBytes.length; i++, j++)
                            {
                                if (i >= totalBytes.length)
                                {
                                    break;
                                }

                                totalBytes[i] = receivedBytes[j];
                            }

                            currentPosition+=receivedBytes.length;
                        }
                    }
                    else
                    {
                        parcel[0] = 0;
                        parcel[1] = command;
                        out.write(parcel);
                    }
                }
                catch (java.io.IOException e) {
                    e.printStackTrace();
                }
                finally {
                    if (socket != null) {
                        socket.close();
                    }
                }
            }
            catch(java.io.IOException e)
            {
                e.printStackTrace();
            }

            return null;
        }

        @Override
        protected void onPostExecute(Void aVoid) {
            try {
                if (isSendingMust) {
                    ImageView image = findViewById(R.id.Screenshot);
                    Bitmap bmp = BitmapFactory.decodeByteArray(totalBytes, 0, totalBytes.length);
                    image.setImageBitmap(Bitmap.createScaledBitmap(bmp, bmp.getWidth(),
                            bmp.getHeight(), false));
                }
            }
            catch (Exception e) {
            }
        }
    }
}
