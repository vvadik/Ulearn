const path = require('path')
const glob = require('webpack-glob-entries')

module.exports = {
  mode: 'development',
  entry: glob(path.resolve(__dirname, 'src', '*.test.js')),
  output: {
    path: path.resolve(__dirname, 'dist', 'src'),
    filename: '[name].js',
  },
  node: false,
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [
              [
                '@babel/preset-env',
                {
                  targets: { node: 'current' },
                },
              ],
            ],
            plugins: ['@babel/plugin-proposal-class-properties'],
          },
        },
      },
    ],
  },
}
