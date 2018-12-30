package com.example.coursework;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.EditText;
import android.view.View;
import android.widget.Toast;

import AdditionalClasses.Matcher;

public class MainActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    public void SetConnection(View view)
    {
        EditText editText = findViewById(R.id.edit_ip);
        String text = editText.getText().toString();

        if (Matcher.IsEnteredIpCorrect(text))
        {
            Intent intent = new Intent(this, ControlActivity.class);
            intent.putExtra("IP", text);
            startActivity(intent);
        }
        else
        {
            Toast toast = Toast.makeText(this,"Wrong ip was entered", Toast.LENGTH_LONG);
            toast.show();
        }
    }
}
