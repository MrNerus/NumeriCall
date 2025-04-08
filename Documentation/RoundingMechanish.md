# Context
Rounding is often done to remove precision. More precise the data, more difficult to calculate. 

Plus, we don't need molecular level precision to make a drinking water bottle. you will splash it out any way. Small dent somewhere and forget that precision. 

# Generally Accepted Rounding Method
Generally, following are accepted rounding method. [Reference](https://www.calculator.net/rounding-calculator.html)
- Round Half Up
- Round Half Down
- Round Half to Even
- Round Half to Odd
- Round half away from zero
- Round Half towards zaro
- Celing
- Floor
- Rounding to Fractions

# Inconsistancy
Lets look at RoundHalfUp

```plotly
ggplot(cars, aes(speed, dist)) + geom_point()
```

0----1----2----3----4----5----6----7----8----9----A