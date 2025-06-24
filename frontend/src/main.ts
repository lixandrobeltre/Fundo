import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

import axios from 'axios';
import { environment } from './environments/environment.development';

axios.defaults.baseURL = environment.apiUrl;

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
