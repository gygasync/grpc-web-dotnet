/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import React, {useState} from 'react';
import {
  Alert,
  Platform,
  SafeAreaView,
  ScrollView,
  StatusBar,
  StyleSheet,
  Text,
  TouchableOpacity,
  useColorScheme,
  View,
} from 'react-native';

import {Colors} from 'react-native/Libraries/NewAppScreen';
import {EchoClient} from './app/api/EchoServiceClientPb';
import {config} from './app.json';
import {Empty} from './app/api/echo_pb';

function App(): JSX.Element {
  const isDarkMode = useColorScheme() === 'dark';

  const backgroundStyle = {
    backgroundColor: isDarkMode ? Colors.darker : Colors.lighter,
  };

  const [streamsOpen, setStreamsOpen] = useState(0);
  const [message, setMessage] = useState('');
  const [streamCallbacks, setStreamCallbacks] = useState([] as {(): void}[]);

  return (
    <SafeAreaView style={backgroundStyle}>
      <StatusBar
        barStyle={isDarkMode ? 'light-content' : 'dark-content'}
        backgroundColor={backgroundStyle.backgroundColor}
      />
      <ScrollView
        contentInsetAdjustmentBehavior="automatic"
        style={backgroundStyle}>
        <View>
          <TouchableOpacity
            onPress={() => {
              const client = new EchoClient(
                Platform.OS === 'android'
                  ? config.android.hostAddress
                  : config.ios.hostAddress,
              );

              const request = new Empty();
              const stream = client.echoStream(request, undefined);

              stream.on('data', data => {
                console.log(data.getMessage());
                Alert.alert(data.getMessage());
                setMessage(data.getMessage());
              });

              stream.on('end', () => {
                setMessage('ENDED STREAM');
              });

              setStreamCallbacks([...streamCallbacks, () => stream.cancel()]);

              setStreamsOpen(streamsOpen + 1);
            }}
            style={styles.touchable}>
            <Text>Open a stream</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.touchable}
            onPress={() => {
              if (streamCallbacks.length > 0) {
                streamCallbacks[0]();
                setStreamCallbacks(streamCallbacks.slice(1));
                setStreamsOpen(streamsOpen - 1);
              }
            }}>
            <Text>Close a stream</Text>
          </TouchableOpacity>
          <TouchableOpacity
            onPress={async () => {
              const client = new EchoClient(
                Platform.OS === 'android'
                  ? config.android.hostAddress
                  : config.ios.hostAddress,
              );
              const request = new Empty();

              const response = await client.echoRequest(request, null);
              Alert.alert(response.getMessage());
            }}
            style={styles.touchable}>
            <Text>Send request</Text>
          </TouchableOpacity>
          <Text>Currently open streams: {streamsOpen}</Text>
          <Text>Stream message: {message}</Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  touchable: {
    padding: 20,
    borderColor: 'black',
    borderWidth: 1,
  },
});

export default App;
