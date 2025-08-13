import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, Button, StyleSheet, Alert, FlatList } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

const API_BASE_URL = process.env.API_BASE_URL || 'http://172.22.18.61:5058';

function LoginScreen({ navigation }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ eposta: email, password })
      });
      if (!response.ok) {
        Alert.alert('Giriş başarısız', 'Kullanıcı bulunamadı.');
        return;
      }
      await response.json();
      navigation.navigate('Home');
    } catch (err) {
      Alert.alert('Hata', err.message);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>BKU Giriş</Text>
      <TextInput
        style={styles.input}
        placeholder="E-posta"
        value={email}
        onChangeText={setEmail}
        autoCapitalize="none"
      />
      <TextInput
        style={styles.input}
        placeholder="Şifre"
        secureTextEntry
        value={password}
        onChangeText={setPassword}
      />
      <Button title="Giriş Yap" onPress={handleLogin} />
    </View>
  );
}

function HomeScreen({ navigation }) {
  return (
    <View style={styles.container}>
      <Button title="Yıl Listesi" onPress={() => navigation.navigate('Years')} />
    </View>
  );
}

function YearListScreen({ navigation }) {
  const years = [2015, 2016, 2017, 2018, 2019, 2020];
  return (
    <View style={styles.container}>
      {years.map(y => (
        <Button key={y} title={String(y)} onPress={() => navigation.navigate('Questions', { year: y })} />
      ))}
    </View>
  );
}

function QuestionScreen({ route }) {
  const { year } = route.params;
  const [questions, setQuestions] = useState([]);

  useEffect(() => {
    const load = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/api/question/year/${year}`);
        const data = await res.json();
        setQuestions(data);
      } catch (err) {
        Alert.alert('Hata', err.message);
      }
    };
    load();
  }, [year]);

  const renderItem = ({ item }) => (
    <View style={styles.question}>
      <Text>{item.text}</Text>
      {item.answers && item.answers.map((ans, idx) => (
        <Text key={idx}>- {ans.text}</Text>
      ))}
    </View>
  );

  return (
    <FlatList
      data={questions}
      keyExtractor={(item) => item.id.toString()}
      renderItem={renderItem}
    />
  );
}

const Stack = createNativeStackNavigator();

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator>
        <Stack.Screen name="Login" component={LoginScreen} />
        <Stack.Screen name="Home" component={HomeScreen} />
        <Stack.Screen name="Years" component={YearListScreen} />
        <Stack.Screen name="Questions" component={QuestionScreen} />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    padding: 20
  },
  title: {
    fontSize: 24,
    marginBottom: 20,
    textAlign: 'center',
    fontWeight: 'bold'
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 10,
    marginBottom: 15,
    backgroundColor: 'white'
  },
  question: {
    padding: 10,
    borderBottomWidth: 1,
    borderColor: '#ddd'
  }
});
