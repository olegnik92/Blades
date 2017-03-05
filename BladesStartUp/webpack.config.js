/// <binding ProjectOpened='Watch - Development' />

"use strict";

var WebpackNotifierPlugin = require('webpack-notifier');
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var extractCSS = new ExtractTextPlugin('./public/stylesheets/[name].bundle.css');

module.exports = {
    entry: {
        app: './frontend/main.ts',
        unitTests: './frontend/unitTests/main.ts'
    },
    output: {
        filename: "./public/javascript/[name].bundle.js"
    },   
    resolve: {
        extensions: [".ts", ".tsx", ".js"]
    },
    module: {
        rules: [
            { test: /\.tsx?$/, loader: 'awesome-typescript-loader' },
            {
                test: /\.css$/,
                loader: extractCSS.extract({
                    use: [{ loader: 'css-loader', options: { modules: true, camelCase: true, localIdentName: '[local]--[hash:base64:10]' } }]
                })
            }
        ]
    },
    plugins: [
        new webpack.NoEmitOnErrorsPlugin(),
        extractCSS,
        new WebpackNotifierPlugin()
    ],
    devtool: "source-map",
};
