import { Component, OnInit, AfterViewInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit, AfterViewInit {
  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.setupDarkModeToggle();
  }

  setupDarkModeToggle() {
    const toggleButton = document.getElementById('darkModeToggle');
    const darkModeText = document.getElementById('darkModeText');
    const body = document.body;

    // Check local storage for dark mode preference
    if (localStorage.getItem('dark-mode') === 'enabled') {
      body.classList.add('dark-mode');
      if (darkModeText) darkModeText.textContent = '☀️ Light Mode';
    }

    // Toggle dark mode on button click
    toggleButton?.addEventListener('click', () => {
      if (body.classList.contains('dark-mode')) {
        body.classList.remove('dark-mode');
        localStorage.setItem('dark-mode', 'disabled');
        if (darkModeText) darkModeText.textContent = '🌙 Dark Mode';
      } else {
        body.classList.add('dark-mode');
        localStorage.setItem('dark-mode', 'enabled');
        if (darkModeText) darkModeText.textContent = '☀️ Light Mode';
      }
    });
  }
}
