import React from 'react'
import sum from '../utils/sum'
import styles from './App.css'

class App extends React.Component {
  state = {
    x: 0,
    y: 0,
  }

  render() {
    const { x, y } = this.state
    return (
      <div className={styles.root}>
        <input
          className={styles.number}
          id="x"
          value={x}
          tabindex="1"
          type="number"
          min="0"
          max="10"
          onChange={this.set('x')}
        />
        <span className={styles.text}>plus</span>
        <input
          className={styles.number}
          id="y"
          value={y}
          tabindex="2"
          type="number"
          min="0"
          max="10"
          onChange={this.set('y')}
        />
        <span className={styles.text}>is</span>
        <span className={styles.result} id="sum">
          {sum(x, y)}
        </span>
      </div>
    )
  }

  set = prop => ({ target: { value } }) =>
    this.setState(() => ({ [prop]: +value || 0 }))
}

export default App
