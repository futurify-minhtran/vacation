/**
 *  Welcome to your gulpfile!
 *  The gulp tasks are splitted in several files in the gulp directory
 *  because putting all here was really too long
 */

'use strict';

var gulp = require('gulp');
var wrench = require('wrench');
var bsConfig = require("gulp-bootstrap-configurator");

/**
 *  This will load all js or coffee files in the gulp directory
 *  in order to load all gulp tasks
 */
wrench.readdirSyncRecursive('./gulp').filter(function(file) {
  return (/\.(js|coffee)$/i).test(file);
}).map(function(file) {
  require('./gulp/' + file);
});


/**
 *  Default task clean temporaries directories and launch the
 *  main optimization build task
 */
gulp.task('default', ['clean'], function () {
  gulp.start('build');
});

// For CSS 
gulp.task('make-bootstrap-css', function(){
  return gulp.src("./config.json")
    .pipe(bsConfig.css())
    .pipe(gulp.dest("./assets"));
    // It will create `bootstrap.css` in directory `assets`. 
});
 
// For JS 
gulp.task('make-bootstrap-js', function(){
  return gulp.src("./config.json")
    .pipe(bsConfig.js())
    .pipe(gulp.dest("./assets"));
    // It will create `bootstrap.js` in directory `assets`. 
});

