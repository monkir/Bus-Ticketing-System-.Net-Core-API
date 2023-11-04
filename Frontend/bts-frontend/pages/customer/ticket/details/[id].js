import axios from "axios";
import { useEffect, useState } from "react";
import { useRouter } from "next/router";
import CustomerHeader from "../../component/header";
import CustomerFooter from "../../component/footer";
import { useForm } from "react-hook-form";
import Link from "next/link";

export default function BookSeat() {
    const router = useRouter()
    const numbers = Array.from({ length: 40 }, (value, index) => index + 1);
    const [data, setData] = useState()
    const [ticketData, setTicketData] = useState();
    const [bookedSeat, setBookedSeat] = useState([]);
    const [ticketSeat, setTicketSeat] = useState([]);
    const [tripId, setTripId] = useState();
    const [message, setMessage] = useState("")
    const { id } = router.query
    async function fetchTicketData() {
        // Fetching ticket Data
        try {
            const response = await axios.get(
                process.env.NEXT_PUBLIC_api_root + '/api/customer/ticket/get/' + id,
                {
                    headers: { 'Authorization': sessionStorage.getItem('token_string') }
                }
            )
            setTicketData(response.data)
            setTicketSeat(response.data.seat_no)
            setTripId(response.data.trip_id)
            console.log(data)
            // console.log(response.data)
        }
        catch (e) {
            try {
                console.log(e);
                setMessage(e.response.data.Message)
            }
            catch {
                console.log(e);
                setMessage("API is not connected")
            }
        }
    }
    async function fetchTripData() {
        // Fetching Trip Data
        try {
            const response = await axios.get(
                process.env.NEXT_PUBLIC_api_root + '/api/customer/trip/' + tripId,
                {
                    headers: { 'Authorization': sessionStorage.getItem('token_string') }
                }
            )
            setData(response.data)
            setBookedSeat(response.data.bookedSeat)
            console.log(data)
            // console.log(response.data)
        }
        catch (e) {
            try {
                console.log(e);
                setMessage(e.response.data.Message)
            }
            catch {
                console.log(e);
                setMessage("API is not connected")
            }
        }
    }
    useEffect(() => {
        if (id !== undefined) {
            fetchTicketData();
            if (tripId !== undefined) {
                fetchTripData();
            }
        }
        // console.log(router.query.id)
    }, [id, tripId])
    return (
        <>
            <CustomerHeader title="Bus Ticketing System" pagename="Customer: Ticket Details" />
            <div className="overflow-x-auto px-10 min-h-[70vh]">
                <div className="flex min-h-full flex-1 flex-col justify-center px-6 py-12 lg:px-8">
                    <div className="mx-auto w-1/2">
                        {
                            data == null
                                ? "Data lis loading"
                                :
                                <ul className="grid gap-6 md:grid-cols-2">
                                    <ul className="my-auto">
                                        <div className="card w-96 bg-zinc-400 shadow-xl">
                                            <div className="card-body text-white">
                                                <h2 className="card-title">Trip Data</h2>
                                                <p>Ticket ID: {data?.id}</p>
                                                <p>Bus ID: {data?.bus_id}</p>
                                                <p>From: {data?.depot?.name}</p>
                                                <p>To: {data?.destination?.name}</p>
                                                <p>Start at: {data?.startTime}</p>
                                                <p>End at: {data?.endTime}</p>
                                                <p>Ticket Status: {data?.status}</p>

                                                <h2 className="card-title">Ticket Data</h2>
                                                <p>Trip ID: {ticketData?.trip_id}</p>
                                                <p>Price: {ticketData?.ammount}</p>
                                                <p>Status: {ticketData?.status}</p>
                                                <p>Cupon: {ticketData?.cupon}</p>
                                                <div className="card-actions justify-start">
                                                    <Link className='btn btn-primary' href={'/customer/ticket'}>Back</Link>
                                                </div>
                                            </div>
                                        </div>


                                    </ul>
                                    <ul className="grid gap-6 md:grid-cols-4">
                                        {numbers.map((i) =>
                                            <li key={i}>
                                                {
                                                    ticketSeat.includes(i)
                                                        ? <input type="checkbox" checked={true} readOnly id={"seat" + i} name="seat_no[]" value={i} className="hidden peer" />
                                                        : bookedSeat.includes(i)
                                                            ? <input type="checkbox" disabled id={"seat" + i} name="seat_no[]" value={i} className="hidden peer" />
                                                            : <input type="checkbox" checked={false} readOnly id={"seat" + i} name="seat_no[]" value={i} className="hidden peer" />
                                                    // <input type="checkbox" id={"seat" + i} name="seat_no[]" value={i} className="hidden peer" checked={true} readOnly
                                                    //     {
                                                    //     ...ticketSeat.includes(i)
                                                    //         ? "checked={true} readOnly"
                                                    //         : bookedSeat.includes(i)
                                                    //             ? "disabled"
                                                    //             : "checked={false} readOnly"
                                                    //     }
                                                    // />
                                                }
                                                <label htmlFor={"seat" + i} className="inline-flex items-center justify-center w-full p-5 text-gray-500 bg-white peer-checked:bg-blue-400 peer-disabled:bg-red-300 border-2 border-gray-200 rounded-lg cursor-pointer dark:hover:text-gray-300 dark:border-gray-700 peer-checked:border-blue-600 hover:text-gray-600 dark:peer-checked:text-gray-300 peer-checked:text-gray-600 hover:bg-gray-50 dark:text-gray-400 dark:bg-gray-800 dark:hover:bg-gray-700">
                                                    <div className="block">
                                                        <div className="w-full text-lg font-semibold">{i}</div>
                                                    </div>
                                                </label>
                                            </li>
                                        )}
                                    </ul>
                                </ul>
                        }
                    </div>

                </div>
            </div>
            <CustomerFooter />
        </>
    )
}