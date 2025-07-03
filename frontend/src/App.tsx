import React from 'react';
import { ReactKeycloakProvider } from '@react-keycloak/web';
import Keycloak, { KeycloakConfig, KeycloakInitOptions  } from 'keycloak-js';
import ReportPage from './components/ReportPage';

const keycloakConfig: KeycloakConfig = {
  url: process.env.REACT_APP_KEYCLOAK_URL,
  realm: process.env.REACT_APP_KEYCLOAK_REALM||"",
  clientId: process.env.REACT_APP_KEYCLOAK_CLIENT_ID||""
};

// Дополнительные опции инициализации с PKCE
const initOptions: KeycloakInitOptions = {
  onLoad: 'login-required',  // или 'check-sso'
  flow: 'standard',          // Включает Authorization Code Flow
  pkceMethod: 'S256',       // Активирует PKCE с методом S256
  enableLogging: true        // Для отладки
};

const keycloak = new Keycloak(keycloakConfig);

const App: React.FC = () => {
  return (
    <ReactKeycloakProvider 
      authClient={keycloak}
      initOptions={initOptions}>
      <div className="App">
        <ReportPage />
      </div>
    </ReactKeycloakProvider>
  );
};

export default App;