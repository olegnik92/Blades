"use strict";

var WebpackNotifierPlugin = require('webpack-notifier');
var webpack = require('webpack');

module.exports = {
    entry: {
        app: './clientModules/main.ts',
    },
    output: {
        filename: "./public/javascript/[name].bundle.js"
    },
    resolve: {
        extensions: [".ts", ".tsx", ".js"],
        modules: ["node_modules", "clientModules"]
    },
    module: {
        rules: [
            { test: /\.tsx?$/, loader: 'awesome-typescript-loader' },
        ]
    },
    plugins: [
        new webpack.NoEmitOnErrorsPlugin(),
        new WebpackNotifierPlugin()
    ],
    devtool: "source-map",
};