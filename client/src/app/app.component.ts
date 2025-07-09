import { Component } from '@angular/core';
import { ViewerComponent } from './components/viewer/viewer.component';

@Component({
  selector: 'app-root',
  imports: [ViewerComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'dicom-viewer';
}
