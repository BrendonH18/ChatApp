const formatDateTime = (dt) => {
	const dateTime = new Date(dt)
	const timeDate_DifferenceInSeconds = Math.ceil((Date.now() - dateTime) / 1000)
	if (timeDate_DifferenceInSeconds <= 59) return `${timeDate_DifferenceInSeconds} second${timeDate_DifferenceInSeconds<=1?"":"s"} ago`
	if (timeDate_DifferenceInSeconds <= 360) return `${Math.floor(timeDate_DifferenceInSeconds / 60)} minutes ago`
	if (timeDate_DifferenceInSeconds <= 60*60*24) return `About ${Math.ceil(timeDate_DifferenceInSeconds / 60 / 60)} hour${Math.ceil(timeDate_DifferenceInSeconds / 60 / 60) <= 1?"":"s"} ago`
	return dateTime.toLocaleString()
}
export default formatDateTime